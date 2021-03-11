using System;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,
            IUserRepository userRepository, IHubContext<PresenceHub> presenceHub,
            PresenceTracker tracker)
        {
            _tracker = tracker;
            _presenceHub = presenceHub;
            _userRepository = userRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
        }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        // Which user profile the currently logged in user has clicked on.
        var otherUser = httpContext.Request.Query["user"].ToString();

        // The group it makes consists of the currently logged in, and the other user you're in the group with.
        // It doesn't matter though who's in the group at that moment, or if none is, etc. It stays just a group.
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(Context, groupName);

        // When a user joins a group, we will do...
        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        // Send the message thread to both of the users
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // When a user connects or disconnects and they are a member of the group, they automatically get removed from that group.
        // One of the advantages of creating a new hub for the messages.
        await RemoveFromMessageGroup(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot send messages to yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found user");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new {username = sender.UserName, knownAs = sender.KnownAs});
            }
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    /* Creating a method that handles creating, adding, and removing users from a Group
       HubCallerContext gives us access to our current Username and ConnectionId.
       We don't actually need this because we are inside the Hub though.

       The result of this, is that we have something to do when a user connects to a hub,
       and the method below is when a user disconnects from a hub.
     */
    private async Task<bool> AddToGroup(HubCallerContext context, string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _messageRepository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup(string connectionId)
    {
        var connection = await _messageRepository.GetConnection(connectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
    }

    // The group name is always gonna be alphabetical order for both the caller and the other.
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
}