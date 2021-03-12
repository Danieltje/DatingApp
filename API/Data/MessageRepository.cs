using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        /* When working with new Entities and methods made for them, we update the DataContext and add DbSets.
           Those are essentially table names that we need to add Groups and Messages related to the Hub.
         */

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            /* If we want to get access to the related entities, we project or we eagerly load the related entities
               We change the FindAsync to SingleOrDefault so we can Include, and then Include the Sender and the Recipient values in the Messages
            */

            return await _context.Messages
            .Include(u => u.Sender)
            .Include(u => u.Recipient)
            .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
                .OrderByDescending(m => m.MessageSent)
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username 
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == 
                    messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };


            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, 
            string recipientUsername)
        {
            // Get the conversation of the users
            var messages = await _context.Messages
                
                // We also removed the includes here because we use Projection now

                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                    && m.Sender.UserName == recipientUsername
                    || m.Recipient.UserName == recipientUsername
                    && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                )
                .OrderBy(m => m.MessageSent)

            // We want to do the projection before we send it to a list.
                .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)

                .ToListAsync();

            // We're making a list of this, because we need to loop over this and any unread messages where the recipient is the 
            // currentUsername, we're gonna mark them as read.
            // Find out if there's any unread messages for the current user they have received
            var unreadMessages = messages.Where(m => m.DateRead == null 
                && m.RecipientUsername == currentUsername).ToList();

            // Mark those messages as read
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            // For query optimization, we don't map down here anymore. 
            return messages;
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

    }
}