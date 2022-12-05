using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.OAuth;
using MKIdentityServer.Models;
using MKIdentityServer.Temp;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http.Headers;

namespace MKIdentityServer.Helpers
{
    public class AuthHandler : IAuthHandler
    {        
        private readonly TokenHandler _tokenHandler;
        private readonly IJsonContext jsonuserContext;
        public AuthHandler(TokenHandler tokenHandler, IJsonContext _jsonuserContext)
        {
            _tokenHandler = tokenHandler;
            jsonuserContext = _jsonuserContext;
        }

        public async Task<SoftBlobsUser> GetUser(AuthDto authDto)
        {
            var sbUser = new SoftBlobsUser();
            
            if ((authDto.Provider?.ToLower() == "google") && !string.IsNullOrEmpty(authDto.IdToken))
            {
                var googleUser = await GetGoogleUser(authDto.IdToken);            
                var user = jsonuserContext.GetUser(googleUser.UserMail);
                if (user == null)
                {
                    user = jsonuserContext.AddGuestGoogleUser(googleUser);                    
                }
                sbUser.UserMail = user.UserMail;
                sbUser.UserName = user.UserName;
                sbUser.userAvatar = user.userAvatar;
                sbUser.Role = user.Role;
                sbUser.accessToken = GetAccessToken(user);
                return sbUser;
            }
            else if(!string.IsNullOrEmpty(authDto.UserName) && !string.IsNullOrEmpty(authDto.Password))
            {
                var user = jsonuserContext.GetUser(authDto.UserName);
                if (user != null && user?.Password == authDto.Password)
                {
                    sbUser.UserMail = user.UserMail;
                    sbUser.UserName = user.UserName;
                    sbUser.userAvatar = user.userAvatar;
                    sbUser.Role = user.Role;
                    sbUser.accessToken = GetAccessToken(user);
                    return sbUser;
                }
            }

            return null;            
        }
        private AppUser? GetUser(string userName)
        {
            return jsonuserContext.GetUser(userName);
        }

        private async Task<SoftBlobsUser> GetGoogleUser(string accToken)
        {
            var sbUser = new SoftBlobsUser();
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
                    sbUser.UserMail = dynamicObject.email;
                    sbUser.UserName = dynamicObject.given_name;
                    sbUser.userAvatar = dynamicObject.picture;
                }
                return sbUser;
            }
           
        }
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(AuthDto authDto)
        {
            //var _goolgeSettings = _configuration.GetSection("GoogleAuthSettings");
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { authDto.Client_Id }
                };
                //var settings = new GoogleJsonWebSignature.ValidationSettings()
                //{
                //    Audience = new List<string>() { _goolgeSettings.GetSection("clientId").Value }
                //};

                var payload = await GoogleJsonWebSignature.ValidateAsync(authDto.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                //log an exception
                return null;
            }
        }
        public string GetAccessToken(AppUser appUser)
        {
            var accessToken = _tokenHandler.GenerateToken(appUser);
            jsonuserContext.UpdateUserAuth(appUser.UserId, accessToken);
            return accessToken;
        }
    }
}
