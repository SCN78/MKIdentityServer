using Google.Apis.Auth;
using MKIdentityServer.Models;
using MKIdentityServer.Temp;

namespace MKIdentityServer.Helpers
{
    public interface IAuthHandler
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(AuthDto externalAuth);
        string GetAccessToken(AppUser appUser);
        Task<SoftBlobsUser> GetUser(AuthDto authDto);
    }
}
