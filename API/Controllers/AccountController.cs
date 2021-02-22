using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {
            // using statement assures when we're finished with the partic HMAC class, it's gonna get disposed of directly
            // it uses the dispose method inside of this class as it should do
            // any class that uses a dispose method, will make use of the IDisposable interface
            // any class that derives from the IDisposable interface/class will have to provide a Dispose method
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
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
    }
}