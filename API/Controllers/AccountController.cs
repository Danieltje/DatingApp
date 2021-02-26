using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        // We inject our TokenService into our AccountController
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // using statement assures when we're finished with the partic HMAC class, it's gonna get disposed of directly
            // it uses the dispose method inside of this class as it should do
            // any class that uses a dispose method, will make use of the IDisposable interface
            // any class that derives from the IDisposable interface/class will have to provide a Dispose method

            // we get access to BadRequest because we are using an ActionResult
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken!");

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
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get the user from our database
            // we use SingleOrDefaultAsync because this method also throws an exception
            // FindAsync is only useful if we're getting something with a primary key
            // It looks if we got a user in our database, or it does not have one
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLowerInvariant());

            if (user == null) return Unauthorized("Invalid username!");

            // doing the reverse we did with the register method
            // we take the key we made in the register. This is the Salt
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // now we need to work out the hash that's in the password in the loginDto
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password!");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            // writing a method to see if there's already an entry in the database with our username
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}