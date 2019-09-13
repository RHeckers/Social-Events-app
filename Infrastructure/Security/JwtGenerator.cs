using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public class JwtGenerator : IJwtGenerator
    {    
        private readonly SymmetricSecurityKey _key;
        public JwtGenerator(IConfiguration configuration)
        {
            // TokenKey only avalible in dev mode
            // Generate key to sign tokens
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        }


        public string CreateToken(AppUser user)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                // Add name as name ID to token
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // Generate signing credentials
            // First create credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Create descriptor and add claims and credentials
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            // Generate and create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Send back the token
            return tokenHandler.WriteToken(token);
        }
    }
}