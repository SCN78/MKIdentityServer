using Newtonsoft.Json;
using System.Security.Cryptography;

namespace MKIdentityServer.Temp
{
    public class JsonUserContext : IJsonContext
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public JsonUserContext(IWebHostEnvironment _hostingEnvironment)
        {
            hostingEnvironment = _hostingEnvironment;
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "UserDBData.json");
            if(!File.Exists(filePath))
            {
                AddDummyData();
            }
        }
        private void AddDummyData()
        {
            var userDBDummy = new UserDB();
            userDBDummy.Users = new List<AppUser>();
            userDBDummy.UserAuths = new List<UserAuth>();
            AppUser userDummy = new AppUser
            {
                UserId = 0,
                UserName = "user@dummy.com",
                Password = "Password123",
                UserSecret = "asdv234234^&%&^%&^hjsdfb2%%%",
                UserMail = "user@dummy.com",
                Role = UserRoles.Guest
            };
            userDBDummy.Users.Add(userDummy);
            UserAuth UserAuthDummy = new UserAuth
            {
                UserId = 0,
                AccessToken = ""
            };
            userDBDummy.UserAuths.Add(UserAuthDummy);

            UpdateUserDB(userDBDummy);
        }

        public AppUser AddGuestGoogleUser(SoftBlobsUser sbUser)
        {
            var userDBDummy = GetUserDB();
            userDBDummy.Users = new List<AppUser>();
            userDBDummy.UserAuths = new List<UserAuth>();
            AppUser userDummy = new AppUser
            {
                UserId = GetUserDB().Users.Count,
                UserName = sbUser.UserName,
                Password = "",
                UserSecret = GetRandomSecretKey(),
                UserMail = sbUser.UserMail,
                userAvatar = sbUser.userAvatar,
                Role = UserRoles.Guest
            };
            userDBDummy.Users.Add(userDummy);
            UpdateUser(userDummy);
            UserAuth UserAuthDummy = new UserAuth
            {
                UserId = userDummy.UserId,
                AccessToken = ""
            };
            userDBDummy.UserAuths.Add(UserAuthDummy);
            UpdateUserAuth(userDummy.UserId, "");
          //  UpdateUserDB(userDBDummy);
            return userDummy;
        }
        private UserDB GetUserDB()
        {            
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "UserDBData.json");           
            string jsontext = File.ReadAllText(filePath);
            var userDB = JsonConvert.DeserializeObject<UserDB>(jsontext);
            return userDB;           
        }

        private void UpdateUserDB(UserDB userDB)
        {
            if(userDB != null)
            {
                var rootPath = hostingEnvironment.ContentRootPath;
                var filePath = Path.Combine(rootPath, "UserDBData.json");
                var dbstring = JsonConvert.SerializeObject(userDB);
                File.WriteAllText(filePath, dbstring);
            }            
        }

        public void UpdateUser(AppUser user)
        {
            var _userDB = GetUserDB();
            _userDB.Users.RemoveAll(r => r.UserId == user.UserId);
            _userDB.Users.Add(user);
            UpdateUserDB(_userDB);
        }

        public void UpdateUserAuth(int userId, string token)
        {
            var _userDB = GetUserDB();
            var _user = _userDB.Users.Where(x => x.UserId == userId).FirstOrDefault();
            if (_user != null)
            {
                _userDB.UserAuths.RemoveAll(r => r.UserId == userId);
                _userDB.UserAuths.Add(new UserAuth
                {
                    UserId = userId,
                    AccessToken = token
                });
            }
            UpdateUserDB(_userDB);
        }

        private string GetRandomSecretKey()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            return Convert.ToBase64String(salt);
        }
        public AppUser? GetUser(string userMail)
        {
            return GetUserDB()?.Users.Where(u => u.UserMail == userMail).FirstOrDefault();
        }

        public string? GetUserSecret(int userId)
        {
            return GetUserDB()?.UserAuths.Where(u => u.UserId == userId).FirstOrDefault()?.AccessToken;
        }
    }
}
