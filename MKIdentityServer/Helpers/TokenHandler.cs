using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using MKIdentityServer.Temp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MKIdentityServer.Helpers
{
    public class TokenHandler
    {
        private readonly IConfiguration _config;
        private readonly IJsonContext _jsonuserContext;
        public TokenHandler(IConfiguration configuration, IJsonContext jsonuserContext)
        {
            _config = configuration;
            _jsonuserContext = jsonuserContext;
        }

        //public string GenerateToken(AppUser appUser)
        //{
        //    var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["JwtToken:SecretKey"]));
        //    var myIssuer = _config["JwtToken:Issuer"];
        //    var myAudience = _config["JwtToken:Audience"];
        //    var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtToken:TokenExpiry"]));

        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim("UserRole", "Administrator"),
        //           // new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        //        }),
        //        Expires = jwtValidity,
        //        Issuer = myIssuer,
        //        Audience = myAudience,
        //        SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}       
        public string GenerateToken(AppUser appUser)
        {           
            var userSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appUser.UserSecret));
            var myIssuer = _config["JwtToken:Issuer"];
            var myAudience = _config["JwtToken:Audience"];
            var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtToken:TokenExpiry"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserRole",appUser.Role.ToString()),                  
                }),
                Expires = jwtValidity,
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(userSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public bool ValidateCurrentToken(string token, AppUser appUser)
        {
            var mySecret = "asdv234234^&%&^%&^hjsdfb2%%%";
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appUser.UserSecret));

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
        private string GetSecretKey(int userId)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
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
