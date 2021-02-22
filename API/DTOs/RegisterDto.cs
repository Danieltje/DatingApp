namespace API.DTOs
{
    public class RegisterDto
    {
        // For DTOs it's not highest priority to call it UserName with the two capitals, it's not as strict as AppUser
        public string Username { get; set; }
        public string Password { get; set; }
    }
}