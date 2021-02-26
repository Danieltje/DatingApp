using System;
using System.Collections.Generic;

namespace API.DTOs
{
    // One of the purposes of a Dto is f.e. prevent the endless loop of object cycle we had in the repo with .Include photos
    // We shape the data more before we write a query for it let's sum it up like that
    public class MemberDto
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string PhotoUrl { get; set; }

        // we removed the passwords in the dto, don't want to send those back with this

        // replaced DateOfBirth; we want to return the age here
        public int Age { get; set; }

        public string KnownAs { get; set; }

        // property that marks the DateTime when their profile got created
        // we removed the initialize part with .Now for this Dto
        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string Introduction { get; set; }   

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        // this will be a collection of photos that the user will most likely store in their profile
        /* We got a relationship between our AppUser and our Photo class
           This type of relationship we call a One to Many; one user can have multiple photos
        */

        /* The problem we have without making a Dto is the endless loop of object cycle
           We changed the ICollection type of Photo to PhotoDto
         */
        public ICollection<PhotoDto> Photos { get; set; }
    }
}