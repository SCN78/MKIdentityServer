using System.Security.Cryptography;

namespace MKIdentityServer.Identity.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserMail { get; set; }
        public UserRoles Role { get; set; } = UserRoles.Guest;
        public string UserSecret { get; set; } = GetRandomSecretKey();
        public string userAvatar { get; set; } = "https://i2.wp.com/www.titanui.com/wp-content/uploads/2015/11/17/Flat-Circular-Unisex-Avatars-Vector.jpg";
        static private string GetRandomSecretKey()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
        }
    }
}
