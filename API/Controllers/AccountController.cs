using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            // using statement assures when we're finished with the partic HMAC class, it's gonna get disposed of directly
            // it uses the dispose method inside of this class as it should do
            // any class that uses a dispose method, will make use of the IDisposable interface
            // any class that derives from the IDisposable interface/class will have to provide a Dispose method

            if(await UserExists(registerDto.Username)) return BadRequest("Username is taken!");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            // Telling EF that we want to add this to our Users collection/table
            // the .Add method means that we're tracking this entity now in EF
            _context.Users.Add(user);

            // here we do call our database and save the user into the users table
            await _context.SaveChangesAsync();

            // return the user
            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            // writing a method to see if there's already an entry in the database with our username
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}