namespace API.DTOs
{

    // The object we're going to return when a user logs in or registers
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }

        // new Dto property to return the Main photo in the nav
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }     
    }
}