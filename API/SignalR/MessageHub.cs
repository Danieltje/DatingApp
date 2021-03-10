using System;
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
        public MessageHub(IMessageRepository messageRepository, IMapper mapper,
            IUserRepository userRepository)
        {
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

        // When a user joins a group, we will do...
        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        // Send the message thread to both of the users
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // When a user connects or disconnects and they are a member of the group, they automatically get removed from that group.
        // One of the advantages of creating a new hub for the messages.
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

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync()) 
        {
            var group = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    // The group name is always gonna be alphabetical order for both the caller and the other.
    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
}