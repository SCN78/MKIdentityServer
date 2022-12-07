using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using MKIdentityServer.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MKIdentityServer.Identity.Helpers
{
    public class TokenHelper
    {
        private readonly IConfiguration _config;
        public TokenHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userSecret, UserRoles roles)
        {
            var userSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(userSecret));
            var myIssuer = _config["JwtToken:Issuer"];
            var myAudience = _config["JwtToken:Audience"];
            var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtToken:TokenExpiry"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserRole",roles.ToString()),
                }),
                Expires = jwtValidity,
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(userSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public bool ValidateCurrentToken(string token, string userSecret)
        {           
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(userSecret));

            var myIssuer = "http://mysite.com";
            var myAudience = "http://myaudience.com";

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = myIssuer,
                    ValidAudience = myAudience,
                    IssuerSigningKey = mySecurityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public string GetClaim(string token, string claimType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            return stringClaimValue;
        }
        private string GenerateHashedPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            var _salt = Convert.ToBase64String(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
