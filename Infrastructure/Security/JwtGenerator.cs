using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public class JwtGenerator : IJwtGenerator
    {
        public string CreateToken(AppUser user)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                // Add name as name ID to token
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            // Generate signing credentials
            // First create key and credentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuPer SeCret KeY For SaltIng the PaasWords"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

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