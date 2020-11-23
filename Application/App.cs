using System;
using System.Linq;
using Domain;
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

        public bool UserExists(long userId)
        {
            var user = database.GetUserById(userId);
            try
            {
                var id = user.Id;
                return true;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public User GetUserById(long userId)
        {
            return database.GetUserById(userId);
        }

        public string GetPlantsByUser(User user)
        {
            return database.GetPlantsByUser(user)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
        }

        public void ChangeUserStatus(long userId, UserStatus newStatus)
        {
            database.UpdateUser(new User(userId) {Status = newStatus});
        }

        public UserStatus GetUserStatus(long userId)
        {
            return database.GetUserById(userId).Status;
        }

        public bool SetNewPlantName(long userId, string plantName)
        {
            if (database.GetPlantsByUser(database.GetUserById(userId)).Any(
                plant => plant.Name.Equals(plantName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            database.UpdateUser(new User(userId) {Status = UserStatus.SendPlantWateringInterval, ActivePlantName = plantName});
            return true;
        }

        public bool DeletePlant(long userId, string plantName)
        {
            if (!database.GetPlantsByUser(database.GetUserById(userId)).Any(
                plant => plant.Name.Equals(plantName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            database.DeletePlant(new Plant(plantName, userId));
            database.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
            return true;
        }
        
        public void AddNewPlantFromActivePlantWithWateringInterval(long userId, int wateringInterval)
        {
            database.AddPlant(new Plant(database.GetUserById(userId).ActivePlantName, userId, wateringInterval));
            database.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
        }

        public void Cancel(long userId)
        {
            database.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
        }
    }
}