using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    /* We're deriving from an Identity IdentityUser class. .NET comes with the Identity packages, so just using it on top.
       The default uses a string as a primary key, but we use an int as a primary key
    */ 
    public class AppUser : IdentityUser<int>
    {
        // property to save the date of birth of the user
        public DateTime DateOfBirth { get; set; }

        // save a nickname for the user what they prefer to be known as
        public string KnownAs { get; set; }

        // property that marks the DateTime when their profile got created
        public DateTime Created { get; set; } = DateTime.Now;

        // a property that marks the DateTime when a user was last active in the application
        public DateTime LastActive { get; set; } = DateTime.Now;

        // property to store the gender of the user in. In most cases Man or Woman
        public string Gender { get; set; }

        // the ability for the user to store a little description introduction for their profile
        public string Introduction { get; set; }   

        // probably so we can specify their gender orientation; like men or women
        public string LookingFor { get; set; }

        // a user can save their interests in their profile
        public string Interests { get; set; }

        // the city the user lives in
        public string City { get; set; }

        // the country the user lives in
        public string Country { get; set; }

        // this will be a collection of photos that the user will most likely store in their profile
        /* We got a relationship between our AppUser and our Photo class
           This type of relationship we call a One to Many; one user can have multiple photos
        */
        public ICollection<Photo> Photos { get; set; }

        // The DateTime data type on it's own doesn't give us an option to calculate the age of a user
        // Going to make a method inside our Entity class. We want to extend the methods inside the DateTime class
        // so we can use that to calculate the age of a user and this is available when we need it
        // it will calculate the age based on the date of birth
        
        // Who has liked the currently logged in user?
        public ICollection<UserLike> LikedByUsers { get; set; }

        // Which users has the current logged in user liked?
        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }

        // This is also specified in AppRole.cs and is part of the relationship.
        // Our AppUserRole is acting as our join table.
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}