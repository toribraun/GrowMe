using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Infrastructure
{
    public interface IUserRepository
    {
        User GetUser(long userId);
    }
    
    public interface IPlantRepository
    {
        Plant GetPlant(long userId);
    }

    public interface IDatabaseTable
    {
        IEnumerable<string[]> GetAllData();
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseTable userTable;

        public UserRepository(IDatabaseTable userTable)
        {
            this.userTable = userTable;
        }
        
        public User GetUser(long userId)
        {
            return userTable.GetAllData()
                .Select(a => new User(long.Parse(a[0]), a[1]))
                .FirstOrDefault(u => u.Id == userId);
        }
    }
}