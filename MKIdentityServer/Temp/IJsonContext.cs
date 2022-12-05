namespace MKIdentityServer.Temp
{
    public interface IJsonContext
    {
        AppUser? GetUser(string userName);
        string? GetUserSecret(int userId);
        AppUser AddGuestGoogleUser(SoftBlobsUser sbUser);
        void UpdateUser(AppUser user);
        void UpdateUserAuth(int userId, string token);
    }
}
