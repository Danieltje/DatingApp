using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        // write the logic so we get the data out of the JSON file and into our database
        // returning void so not giving our Task any type parameter (not returning anything)
        public static async Task SeedUsers(DataContext context)
        {
            // check if our Users table contains any users. Returns if it has any users
            if (await context.Users.AnyAsync()) return;

            /* Continues if not any users.... */

            // read the file json file we created with the seed data and store it in userData
            // userData will become a string of JSON text
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

            // here we Deserialize what's inside userData, and turn it into an object
            // users should be a normal List of type User
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // we're seeding each user in the list we created/deserialized
            // we turn all the UserNames into lowercase and create a password which is hashed
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLowerInvariant();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

            // remember Add just adds tracking to the user for EF
                context.Users.Add(user);
            }

            // saving the changes to the database
            await context.SaveChangesAsync();
        }
    }
}