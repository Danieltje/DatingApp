namespace API.Entities
{
    public class Connection
    {
        // This is an Entity, so we need a default constructor for EF to prevent an error.
        public Connection()
        {
        }

        // When we create a new instance of this Connection we need to pass in those 2 properties.
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        // By giving it the name, and Id, EF will recognize it as the primary key.
        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}