using MKIdentityServer.Context;
using MKIdentityServer.Identity.Helpers;
using MKIdentityServer.Identity.Models;
using MKIdentityServer.Models;
using MKIdentityServer.Temp;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MKIdentityServer.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IDataContext dataContext;
        private readonly TokenHelper tokenHelper;
        public AuthService(IDataContext dataContext, TokenHelper tokenHelper)
        {
            this.dataContext = dataContext;
            this.tokenHelper = tokenHelper;
        }
        public async Task<object> GetAuthorizedUser(AuthDto authDto)
        {            
            if (authDto.Provider?.ToLower() == "google" && !string.IsNullOrEmpty(authDto.IdToken))
            {
                var googleUser = await ValidateGoogleUser(authDto.IdToken);
                if(dataContext.GetUser(googleUser.UserMail) == null)
                    dataContext.AddAsGuestUser(googleUser);

                return FetchUser(googleUser);
                              
            }
            else if (!string.IsNullOrEmpty(authDto.UserName) && !string.IsNullOrEmpty(authDto.Password))
            {
                var normalUser = dataContext.GetUser(authDto.UserName);
                if (normalUser != null && normalUser?.Password == authDto.Password)
                {
                    return FetchUser(normalUser);
                }
            }
            return null;
        }
        private object FetchUser(User user)
        {
            var dbUser = dataContext.GetUser(user.UserMail);
            return new
            {
                UserMail = dbUser.UserMail,
                UserName = dbUser.UserName,
                userAvatar = dbUser.userAvatar,
                Role = dbUser.Role,
                accessToken = GetAccessToken(dbUser)
            };
        }
        private async Task<User> ValidateGoogleUser(string accToken)
        {
            var _gUser = new User();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v3/");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accToken);
                HttpResponseMessage response = await httpClient.GetAsync("userinfo");
                if (response.IsSuccessStatusCode)
                {
                    var googleResponse = await response.Content.ReadAsStringAsync();
                    var dynamicObject = JsonConvert.DeserializeObject<dynamic>(googleResponse);
                    _gUser.UserMail = dynamicObject.email;
                    _gUser.UserName = dynamicObject.given_name;
                    _gUser.userAvatar = dynamicObject.picture;
                }
                return _gUser;
            }

        }

        private string GetAccessToken(User user)
        {
            var token = tokenHelper.GenerateToken(user.UserSecret, user.Role);
            dataContext.UpdateUserAuth(user.UserId, token);
            return token;
        }
    }
}
