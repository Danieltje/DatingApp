using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // implement the ITokenService interface
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        // We need a constructor inside here because we need to inject our configuration in this class
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // Adding our claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            // The roles this user belongs to
            var roles = await _userManager.GetRolesAsync(user);

            // Add those roles to the claims
            // Selecting a role from a list of roles. Using ClaimTypes has an option for a role
            // We add those claims to our token
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Creating credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Describing how the token's gonna look
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // A tokenhandler, just needs to be created so we can access the CreateToken method with 
            // the Descriptor as a parameter. We can also access WriteToken method then to return the actual token
            var tokenHandler = new JwtSecurityTokenHandler();


            // Actually create the token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Now write the token and return it so it's created
            return tokenHandler.WriteToken(token);
        }
    }
}