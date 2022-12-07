using MKIdentityServer.Identity.Models;

namespace MKIdentityServer.Context
{
    public interface IDataContext
    {
        User? GetUser(string userName);
        User? GetUser(int userId); 
        void UpdateUser(User user);
        void UpdateUserAuth(int userId, string token);
        void AddUser(User user);
        void AddAuth(Auth auth);
        void AddAsGuestUser(User sbUser);
    }
}
