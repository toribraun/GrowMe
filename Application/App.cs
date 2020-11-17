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
    }
}