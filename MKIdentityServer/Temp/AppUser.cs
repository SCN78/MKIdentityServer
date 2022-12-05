namespace MKIdentityServer.Temp
{
    public class AppUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserMail { get; set; }
        public UserRoles Role { get; set; } = UserRoles.Guest;
        public string UserSecret { get; set; }
        public string userAvatar { get; set; } = "https://i2.wp.com/www.titanui.com/wp-content/uploads/2015/11/17/Flat-Circular-Unisex-Avatars-Vector.jpg";
    }
}
