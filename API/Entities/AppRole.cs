using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        // A Collection of all the roles the AppUser has
        // A part of the relationship we need to complete the relationships we need for these entities
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}