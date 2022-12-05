namespace MKIdentityServer.Temp
{
    public interface IUserContext
    {
        UserDB GetUserDB();
        void UpdateUserDB(UserDB userDB);
    }
}
