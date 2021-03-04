using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{

    // A good place to add validation is in the DTO. The controller uses those properties and we want to validate what's inside there
    // This also has to do with the properties of an ApiController. It has some built in features to help with validation to sum it up
    public class RegisterDto
    {
        // For DTOs it's not highest priority to call it UserName with the two capitals, it's not as strict as AppUser
        // Data annotations
        [Required] public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }
    }
}