using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        // A Dictionary stores key and value pairs.
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId)
        {
            // Locking the Dictionary till we are finished what we're doing inside here.
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else 
                {
                    OnlineUsers.Add(username, new List<string>{connectionId});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                }
            }

            return Task.CompletedTask;
        }

        /* This all happens in memory. Our Dictionary is gonna be stored in memory, and not in our database. */
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        /* Create a method that can get a list of Connections for a particular User that's stored inside the Dictionary  */
        public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock(OnlineUsers)
            {
                // If we have a Dictionary element with a key of username, return the connectionIds of that particular user.
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }
    }
}