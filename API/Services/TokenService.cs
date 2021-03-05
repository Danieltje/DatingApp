using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // implement the ITokenService interface
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        // We need a constructor inside here because we need to inject our configuration in this class
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // Adding our claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

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