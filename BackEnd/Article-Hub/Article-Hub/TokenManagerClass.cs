using Article_Hub.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Article_Hub
{
    public class TokenManagerClass
    {
        public static string Secret = "cab586fdd8bee3c62335f7215eed1e342b3941ddfaecc892bfdca53c6d722d99eea159593033576e34d4e7f17e0c6c697db1e4a4f004a2755a985859c3a251de";

        public static string GenerateToken(string email, string isDeletable)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email), new Claim("isDeletable", isDeletable) }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtSecurityToken == null)
                {
                    return null;
                }
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret))
                };

                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                return principal;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static tokenClaim ValidateToken(string rawToken)
        {
            tokenClaim tokenClaim = new tokenClaim();
            string[] array = rawToken.Split(' ');
            var token = array[1];
            ClaimsPrincipal principal = GetPrincipal(token);
            Claim email = principal.FindFirst(ClaimValueTypes.Email);
            tokenClaim.Email = email.Value;
            Claim isDeletable = principal.FindFirst("isDeletable");
            tokenClaim.isDeletable = isDeletable.Value;
            return tokenClaim;

        }


    }
}