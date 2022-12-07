using MKIdentityServer.Context;
using MKIdentityServer.Identity.Helpers;
using MKIdentityServer.Identity.Services;

namespace MKIdentityServer.Helpers
{
    public static class ServiceExtensions
    {
        public static void RegisterRepos(this IServiceCollection collection)
        {   
            collection.AddTransient<IAuthService, AuthService>();
            collection.AddTransient<IDataContext, JsonDbContext>();
            collection.AddTransient<TokenHelper>();
        }
    }
}
