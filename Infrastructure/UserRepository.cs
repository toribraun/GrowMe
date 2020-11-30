using System.Linq;

namespace Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseTable<UserRecord> userTable;

        public UserRepository(IDatabaseTable<UserRecord> userTable)
        {
            this.userTable = userTable;
        }
        
        public UserRecord GetUser(long userId)
        {
            return userTable.GetAllData()
                .FirstOrDefault(u => u.Id == userId);
        }

        public void UpdateUser(UserRecord newUser)
        {
            var users = userTable.GetAllData().ToList();
            for (var i = 0; i < users.Count; i++)
            {
                if (!users[i].Equals(newUser)) 
                    continue;
                newUser.Name ??= users[i].Name;
                users[i] = newUser;
                break;
            }
            userTable.WriteAllData(users);
        }

        public void AddNewUser(UserRecord user)
        {
            userTable.AddNewRecord(user);
        }
    }
}