namespace Infrastructure
{
    public interface IUserRepository
    {
        UserRecord GetUser(long userId);
        void UpdateUser(UserRecord newUser);
        void AddNewUser(UserRecord newUser);
    }
}