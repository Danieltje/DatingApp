using System.Threading.Tasks;
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
        public MessageHub(IMessageRepository messageRepository, IMapper mapper)
        {
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
        }

        // The group name is always gonna be alphabetical order for both the caller and the other.
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}