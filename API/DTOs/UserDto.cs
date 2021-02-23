namespace API.DTOs
{

    // The object we're going to return when a user logs in or registers
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }     
    }
}