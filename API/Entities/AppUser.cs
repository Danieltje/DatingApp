namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        // Storing the hashed password as a byte array
        public byte[] PasswordHash { get; set; }

        // Storing the salted password as a byte array
        public byte[] PasswordSalt { get; set; }
    }
}