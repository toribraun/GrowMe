using System;
using System.Linq;
using System.Threading;
using Domain;
using Infrastructure;

namespace Application
{
    public class App
    {
        private IUserRepository userRepository;
        private IPlantRepository plantRepository;
        private Timer timer;
        public event Action<long, string> SendNotification;

        public App()
        {
            userRepository = new UserRepository(new DatabaseCsvTable<User>("users.csv"));
            plantRepository = new PlantRepository(new DatabaseCsvTable<Plant>("users_plants.csv"));
            var tm = new TimerCallback(SendNotifications);
            timer = new Timer(tm, new object(), 0, 1000 * 3600 * 3);
        }

        public void SendNotifications(object obj)
        {
            foreach (var plant in plantRepository.GetPlantsToWater())
            {
                SendNotification?.Invoke(plant.UserId, plant.Name);
                Console.WriteLine($"userId {plant.UserId}, {plant.Name}");
            }
        }
        
        public bool AddUser(User user)
        {
            if (userRepository.GetUser(user.Id) == null)
                return false;
            userRepository.AddNewUser(user);
            return true;
        }

        public bool UserExists(long userId)
        {
            var user = userRepository.GetUser(userId);
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
            return userRepository.GetUser(userId);
        }

        public string GetPlantsByUser(User user)
        {
            return plantRepository.GetPlantsByUser(user.Id)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
        }

        public void ChangeUserStatus(long userId, UserStatus newStatus)
        {
            userRepository.UpdateUser(new User(userId) {Status = newStatus});
        }

        public UserStatus GetUserStatus(long userId)
        {
            return userRepository.GetUser(userId).Status;
        }

        public bool SetNewPlantName(long userId, string plantName)
        {
            if (plantRepository.GetPlantsByUser(userRepository.GetUser(userId).Id).Any(
                plant => plant.Name.Equals(plantName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            userRepository.UpdateUser(new User(userId)
            {
                Status = UserStatus.SendPlantWateringInterval, 
                ActivePlantName = plantName
            });
            return true;
        }

        public bool DeletePlant(long userId, string plantName)
        {
            if (!plantRepository.GetPlantsByUser(userRepository.GetUser(userId).Id).Any(
                plant => plant.Name.Equals(plantName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            plantRepository.DeletePlant(new Plant(plantName, userId));
            userRepository.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
            return true;
        }
        
        public void AddNewPlantFromActivePlantWithWateringInterval(long userId, int wateringInterval)
        {
            plantRepository.AddNewPlant(new Plant(userRepository.GetUser(userId).ActivePlantName, userId, wateringInterval));
            userRepository.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
        }

        public void Cancel(long userId)
        {
            userRepository.UpdateUser(new User(userId) {Status = UserStatus.DefaultStatus});
        }
    }
}