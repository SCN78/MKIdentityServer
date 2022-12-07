using MKIdentityServer.Identity.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace MKIdentityServer.Context
{
    public class JsonDbContext : IDataContext
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public JsonDbContext(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            var rootPath = hostingEnvironment.ContentRootPath;
            var usersFilePath = Path.Combine(rootPath, "UserData.json");
            var authFilePath = Path.Combine(rootPath, "AuthData.json");
            if (!File.Exists(usersFilePath))
            {
                AddDummyUser();
            }
            if (!File.Exists(authFilePath))
            {
                AddDummyAuth();
            }
        }

        private void AddDummyUser()
        {
            var usrList = new List<User>();            
            var usr = new User
            {
                UserId = 0,
                UserName = "user@dummy.com",
                Password = "Password123",
                UserSecret = "asdv234234^&%&^%&^hjsdfb2%%%",
                UserMail = "user@dummy.com",
                Role = UserRoles.Guest
            };
            usrList.Add(usr);
            Save(usrList);
        }

        private void AddDummyAuth()
        { 
            var usrAuths = new List<Auth>();
            var auth = new Auth
            {
                UserId = 0,
                AccessToken = ""
            };

            usrAuths.Add(auth);
            Save(usrAuths);
        }
       
        private List<User> GetAllUsers()
        {
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "UserData.json");
            string jsontext = File.ReadAllText(filePath);
            var usersList = JsonConvert.DeserializeObject<List<User>>(jsontext);
            return usersList;
        }

        private List<Auth> GetAllAuths()
        {
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "AuthData.json");
            string jsontext = File.ReadAllText(filePath);
            var authList = JsonConvert.DeserializeObject<List<Auth>>(jsontext);
            return authList;
        }

        private void Save(List<User> userList)
        {
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "UserData.json");
            var dbstring = JsonConvert.SerializeObject(userList);
            File.WriteAllText(filePath, dbstring);
        }

        private void Save(List<Auth> authList)
        {
            var rootPath = hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(rootPath, "AuthData.json");
            var dbstring = JsonConvert.SerializeObject(authList);
            File.WriteAllText(filePath, dbstring);
        }

        public void AddAsGuestUser(User sbUser)
        {
            sbUser.UserId = GetAllUsers().Count;           
            AddUser(sbUser);
        }

        public void AddUser(User user)
        {
            if (user != null)
            {
                var users = GetAllUsers();
                users.RemoveAll(r => r.UserId == user.UserId);
                users.Add(user);
                Save(users);
            }
        }

        public void AddAuth(Auth auth)
        {
            if (auth != null)
            {
                var auths = GetAllAuths();
                auths.RemoveAll(r => r.UserId == auth.UserId);
                auths.Add(auth);
                Save(auths);
            }
        }

        public User? GetUser(string userName)
        {
            return GetAllUsers().Where(x => x.UserMail == userName).FirstOrDefault();
        }

        public User? GetUser(int userId)     
        {
            return GetAllUsers().Where(x => x.UserId == userId).FirstOrDefault();
        }

        public void UpdateUser(User user)
        {
            AddUser(user);
        }

        public void UpdateUserAuth(int userId, string token)
        {
            AddAuth(new Auth
            {
                UserId = userId,
                AccessToken = token
            });
        }       
    }
}
