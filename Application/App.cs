﻿using System;
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

        public App(IUserRepository userRepository, IPlantRepository plantRepository)
        {
            this.userRepository = userRepository;
            this.plantRepository = plantRepository;
            InitTimer();
        }

        private void InitTimer()
        {
            var tm = new TimerCallback(SendNotifications);
            timer = new Timer(tm, new object(), 0, 1000 * 3600 * 3);
        }

        public void SendNotifications(object obj)
        {
            foreach (var plant in plantRepository.GetPlantsToWater())
            {
                SendNotification?.Invoke(plant.UserId, plant.Name);
            }
        }
        
        private User UserRecordToUser(UserRecord userRecord) => new User(
            userRecord.Id, 
            userRecord.Name) 
            {Status = (UserStatus)(int)userRecord.Status, ActivePlantName = userRecord.ActivePlantName};
        
        private PlantRecord PlantToPlantRecord(Plant plant) => new PlantRecord(
            plant.Name, 
            plant.UserId, 
            plant.WateringInterval)
        {
            NextWateringTime = plant.NextWateringTime,
            WateringStatus = plant.WateringStatus,
            AddingDate = plant.AddingDate,
            ShouldBeDeleted = plant.ShouldBeDeleted
        };
        
        public bool AddUser(long userId, string userName)
        {
            if (userRepository.GetUser(userId) != null)
                return false;
            userRepository.AddNewUser(new UserRecord(userId, userName));
            return true;
        }

        public bool UserExists(long userId)
        {
            if (userRepository.GetUser(userId) == null)
                return false;
            return true;
        }

        public User GetUserById(long userId)
        {
            var userRecord = userRepository.GetUser(userId);
            return new User(userRecord.Id, userRecord.Name);
        }

        public void GetPlantsByUserEvent(long userId)
        {
            var plants = plantRepository.GetPlantsByUser(userId)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
            OnReply.Invoke(new Reply() {Text = plants, Type = ReplyType.GetPlantsByUser});
            
        }
        
        public string GetPlantsByUser(User user)
        {
            return plantRepository.GetPlantsByUser(user.Id)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
            
        }

        public class Reply : IReply
        {
            public long UserId { get; }
            public string Text { get; set; }
            public ReplyType Type { get; set; }
        }

        public enum ReplyType
        {
            GetPlantsByUser
        }

        public event Action<Reply> OnReply;

        public void ChangeUserStatus(long userId, UserStatus newStatus)
        {
            userRepository.UpdateUser(new UserRecord(userId) {Status = (UserStatusRecord)(int)newStatus});
        }

        public UserStatus GetUserStatus(long userId)
        {
            return UserRecordToUser(userRepository.GetUser(userId)).Status;
        }

        public bool SetNewPlantName(long userId, string plantName)
        {
            if (plantRepository.GetPlantsByUser(userRepository.GetUser(userId).Id).Any(
                plant => plant.Name.Equals(plantName, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            userRepository.UpdateUser(new UserRecord(userId)
            {
                Status = UserStatusRecord.SendPlantWateringInterval, 
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
            plantRepository.DeletePlant(PlantToPlantRecord(new Plant(plantName, userId)));
            userRepository.UpdateUser(new UserRecord(userId) {Status = UserStatusRecord.DefaultStatus});
            return true;
        }
        
        public void AddNewPlantFromActivePlantWithWateringInterval(long userId, int wateringInterval)
        {
            plantRepository.AddNewPlant(PlantToPlantRecord(new Plant(userRepository.GetUser(userId).ActivePlantName, userId, wateringInterval)));
            userRepository.UpdateUser(new UserRecord(userId) {Status = UserStatusRecord.DefaultStatus});
        }

        public void Cancel(long userId)
        {
            userRepository.UpdateUser(new UserRecord(userId) {Status = UserStatusRecord.DefaultStatus});
        }
    }
}