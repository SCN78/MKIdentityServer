namespace MKIdentityServer.Temp
{
    public class SoftBlobsUser
    {
        public string userAvatar { get; set; }
        public string UserName { get; set; }
        public string UserMail{get; set;}
        public UserRoles Role { get; set; } = UserRoles.Guest;
        public string accessToken { get; set; }
        
    }
}
