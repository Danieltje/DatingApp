namespace API.DTOs
{
    // we receive data from our client/user and we need to handle this update also in the API
    // here we specify the fields that the user is allowed to update with the update function
    public class MemberUpdateDto
    {
        // we are not adding validation here because a user can also remove all of their input data f.e.
        // they are free to chose which fields they populate and how they populate them

        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // we now want to map this into our User entity
    }
}