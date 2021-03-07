namespace API.DTOs
{
    public class LikeDto
    {
        // The information we need to display a member card, just like we did in the members list.
        // The users we like are gonna be displayed as a card is the idea.

        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}