using Microsoft.IdentityModel.Tokens;
using MKIdentityServer.Temp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    }
}
