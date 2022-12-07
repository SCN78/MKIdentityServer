using MKIdentityServer.Identity.Models;


namespace MKIdentityServer.Identity.Services
{
    public interface IAuthService
    {
        Task<object> GetAuthorizedUser(AuthDto authDto);
    }
}
