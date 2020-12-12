using System;
using System.Linq;
using System.Threading;
using Application.Replies;
using Domain;
using Infrastructure;

namespace Application
{
    public class App
    {
        private IUserRepository userRepository;
        private IPlantRepository plantRepository;
        private Timer timer;
        public event EventHandler<EventArgsSendNotifications> SendNotification;
        public event EventHandler<IReply> OnReply; 

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

        public class EventArgsSendNotifications
        {
            public long UserId { get; }
            public string PlantName { get; }
            public EventArgsSendNotifications(long userId, string plantName)
            {
                UserId = userId;
                PlantName = plantName;
            }
        }

        public void SendNotifications(object obj)
        {
            foreach (var plant in plantRepository.GetPlantsToWater())
            {
                SendNotification?.Invoke(this, new EventArgsSendNotifications(plant.UserId, plant.Name));
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
            ShouldBeDeleted = plant.ShouldBeDeleted,
            FirstPhotoId = plant.FirstPhotoId,
            LastPhotoId = plant.LastPhotoId,
        };
        
        private Plant PlantRecordToPlant(PlantRecord plant) => new Plant(
            plant.Name, 
            plant.UserId, 
            plant.WateringInterval)
        {
            NextWateringTime = plant.NextWateringTime,
            WateringStatus = plant.WateringStatus,
            AddingDate = plant.AddingDate,
            ShouldBeDeleted = plant.ShouldBeDeleted,
            FirstPhotoId = plant.FirstPhotoId,
            LastPhotoId = plant.LastPhotoId,
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

        public void GetHelp(long userId)
        {
            OnReply?.Invoke(this, new ReplyOnHelp(userId));
        }

        public void HandleNonexistingCommand(long userId, string message)
        {
            var status = GetUserStatus(userId);
            switch (status)
            {
                case UserStatus.GetPlantsNames:
                {
                    var existingPlants = GetPlantsByUser(new User(userId));
                    if (existingPlants.Contains(message))
                    {
                        var plant = GetPlantByUser(new User(userId), message);
                        if (string.IsNullOrEmpty(plant.FirstPhotoId))
                            OnReply?.Invoke(this, 
                                new ReplyOnGetPlantPhoto(userId,false));
                        else
                        {
                            OnReply?.Invoke(
                                this, 
                                new ReplyOnGetPlantPhoto(userId, 
                                    plant.Name, plant.FirstPhotoId, plant.LastPhotoId, plant.AddingDate, true));
                        }
                    }

                    break;
                }
                case UserStatus.SendPlantName:
                {
                    var input = message.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                    var existingPlants = GetPlantsByUser(new User(userId));
                    if (input.Length != 1 || input[0].Length > 25 || existingPlants.Contains(input[0]))
                    {
                        OnReply?.Invoke(this, new ReplyOnWantedAddPlant(userId, true));
                        return;
                    }    
                    SetNewPlantName(userId, message);
                    OnReply?.Invoke(this, new ReplyOnSetPlantName(userId));
                    break;
                }
                case UserStatus.SendPlantWateringInterval:
                {
                    if (!int.TryParse(message, out var interval) || interval <= 0)
                    {
                        OnReply?.Invoke(this, new ReplyOnSetPlantName(userId, true));
                        return;
                    }
                    AddNewPlantFromActivePlantWithWateringInterval(userId, interval);
                    OnReply?.Invoke(this, new ReplyOnSetWateringInterval(userId));
                    break;
                }
                case UserStatus.DeletePlantByName:
                    OnReply?.Invoke(this, new ReplyOnDeletedPlant(userId, message, DeletePlant(userId, message)));
                    break;
                case UserStatus.DefaultStatus:
                    break;
                case UserStatus.SendUserName:
                    break;
                default:
                    OnReply?.Invoke(this, new ReplyOnNotCommand(userId));
                    break;
            }
        }
        
        public void CheckUserExistEvent(long userId, string userName)
        {
            var isAdded = AddUser(userId, userName);
            if (isAdded)
                OnReply?.Invoke(this, new ReplyOnStart(userId, userName, isAdded));
        }
        
        public void StartEvent(long userId, string userName)
        {
            OnReply?.Invoke(this, new ReplyOnStart(userId, userName, AddUser(userId, userName)));
        }

        public void GetPlantsByUserEvent(long userId)
        {
            var plants = plantRepository.GetPlantsByUser(userId)
                .Select(record => record.Name);
            ChangeUserStatus(userId, UserStatus.GetPlantsNames);
            OnReply?.Invoke(this, new ReplyOnGetPlants(userId, plants));
        }

        public void GetPlantsToDeleteEvent(long userId)
        {
            var plants = plantRepository.GetPlantsByUser(userId)
                .Select(record => record.Name);
            if (plants.Any()) 
                ChangeUserStatus(userId, UserStatus.DeletePlantByName);
            OnReply?.Invoke(this, new ReplyOnGetPlantsToDelete(userId, plants));
        }
        
        public void AddPlantByUserEvent(long userId)
        {
            ChangeUserStatus(userId, UserStatus.SendPlantName);
            OnReply?.Invoke(this, new ReplyOnWantedAddPlant(userId));
        }
        
        public void AddPlantPhotoEvent(long userId, string plantName, string photoId)
        {
            var isAdded = AddPhoto(userId, plantName, photoId);
            OnReply?.Invoke(this, new ReplyOnSetPlantPhoto(userId, plantName, isAdded));
        }

        public string GetPlantsByUser(User user)
        {
            return plantRepository.GetPlantsByUser(user.Id)
                .Aggregate("", (message, plant) => message + $"{plant.Name}\n");
        }
        
        public Plant GetPlantByUser(User user, string plantName)
        {
            return PlantRecordToPlant(plantRepository
                .GetPlantsByUser(user.Id)
                .FirstOrDefault(p => p.Name == plantName));
        }

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

        public bool AddPhoto(long userId, string plantName, string photoId)
        {
            var currentPlant = plantRepository
                .GetPlantsByUser(userId)
                .FirstOrDefault(p => p.Name == plantName);
            if (currentPlant == null) 
                return false;
            if (string.IsNullOrEmpty(currentPlant.FirstPhotoId))
                currentPlant.FirstPhotoId = photoId;
            else
                currentPlant.LastPhotoId = photoId;
            plantRepository.UpdatePlant(currentPlant);
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
            OnReply?.Invoke(this, new ReplyOnCancel(userId));
        }
    }
}