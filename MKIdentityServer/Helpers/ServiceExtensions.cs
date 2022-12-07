using MKIdentityServer.Context;
using MKIdentityServer.Identity.Helpers;
using MKIdentityServer.Identity.Services;
using MKIdentityServer.Temp;

namespace MKIdentityServer.Helpers
{
    public static class ServiceExtensions
    {
        public static void RegisterRepos(this IServiceCollection collection)
        {            
            collection.AddTransient<IAuthHandler, AuthHandler>();
            collection.AddTransient<TokenHandler>();
            collection.AddTransient<IJsonContext, JsonUserContext>();
            collection.AddTransient<IAuthService, AuthService>();
            collection.AddTransient<IDataContext, JsonDbContext>();
            collection.AddTransient<TokenHelper>();
        }
    }
}
