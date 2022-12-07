using MKIdentityServer.Identity.Models;
using MKIdentityServer.Models;


namespace MKIdentityServer.Identity.Services
{
    public interface IAuthService
    {
        Task<object> GetAuthorizedUser(AuthDto authDto);
    }
}
