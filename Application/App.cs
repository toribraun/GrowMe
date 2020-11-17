using System.Linq;
using Infrastructure;

namespace Application
{
    public class App
    {
        private IDataBase database;

        public App()
        {
            database = new CsvDatabase(
                "users.csv", "users_plants.csv", "common_plants.csv");
        }
        
        public bool AddUser(User user)
        {
            if (database.GetUsers().Contains(user))
                return false;
            database.AddUser(user);
            return true;
        }

        public string GetPlantsByUser(User user)
        {
            return database.GetPlantsByUser(user)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
        }

        public void ChangeUserStatus(long userId, UserStatus newStatus)
        {
            database.UpdateUserStatus(new User(userId), newStatus);
        }

        public UserStatus GetUserStatus(long userId)
        {
            return database.GetUserStatus(new User(userId));
        }

        public void GetNewPlantName(long userId, string plantName)
        {
            database.UpdateUserStatus(new User(userId), UserStatus.SendPlantWateringInterval);
            database.UpdateUsersActivePlant(new User(userId), plantName);
        }
        
        public void AddNewPlantFromActivePlantWithWateringInterval(long userId, int wateringInterval)
        {
            
        }
    }
}