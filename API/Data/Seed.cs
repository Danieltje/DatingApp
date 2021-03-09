using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        // We're gonna use UserManager now to create our Users instead
        public static async Task SeedUsers(UserManager<AppUser> userManager, 
            RoleManager<AppRole> roleManager)
        {
            // check if our Users table contains any users. Returns if it has any users
            if (await userManager.Users.AnyAsync()) return;

            /* Continues if not any users.... */

            // read the file json file we created with the seed data and store it in userData
            // userData will become a string of JSON text
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            // here we Deserialize what's inside userData, and turn it into an object
            // users should be a normal List of type User
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            if (users == null) return;

            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // we're seeding each user in the list we created/deserialized
            // we turn all the UserNames into lowercase and create a password which is hashed
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLowerInvariant();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator"});
        }
    }
}