#nullable enable
using System;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper.Configuration.Attributes;

namespace Infrastructure
{
    public interface IDataBase
    {
        void AddUser(User user);
        void AddPlant(Plant plant);
        IEnumerable<User> GetUsers();
        IEnumerable<Plant> GetPlantsByUser(User user);
        User GetUserById(long id);
        void UpdateUser(User currentUser);
        void UpdatePlant(Plant currentPlant);
        void DeletePlant(Plant currentPlant);
    }

    public enum UserStatus
    {
        DefaultStatus,
        SendUserName,
        SendPlantName,
        SendPlantWateringInterval,
        DeletePlantByName
    }

    public class User
    {
        [Name("id")]
        public long Id { get; set; }
        [Name("name")]
        public string Name { get; set; }
        [Name("status")]
        public UserStatus Status { get; set; }
        [Name("activePlantName")]
        public string ActivePlantName { get; set; }
        
        public User() {}
        public User(long id)
        {
            Id = id;
        }

        public User(long id, string name)
        {
            Id = id;
            Name = name;
            Status = UserStatus.DefaultStatus;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Id == ((User)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class Plant
    {
        [Name("name")]
        public string Name { get; set; }
        [Name("userId")]
        public long UserId { get; set; }

        private int wateringInterval;
        [Name("wateringInterval")]
        public int WateringInterval {
            get => wateringInterval;
            set
            {
                if (value <= 0)
                    throw new ArgumentException();
                wateringInterval = value;
            } }
        [Name("nextWateringTime")]
        public DateTime NextWateringTime { get; set; }
        [Name("wateringStatus")]
        public bool WateringStatus { get; set; }
        [Name("addingDate")]
        public DateTime AddingDate { get; set; }
        [Name("shouldBeDeleted")]
        public bool ShouldBeDeleted { get; set; }
        
        public Plant() {}

        public Plant(string name, long userId)
        {
            Name = name;
            UserId = userId;
        }

        public Plant(string name, long userId, int wateringInterval)
        {
            Name = name;
            UserId = userId;
            WateringInterval = wateringInterval;
            AddingDate = DateTime.Today;
            NextWateringTime = DateTime.Now.AddDays(WateringInterval);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Name == ((Plant) obj).Name && UserId == ((Plant) obj).UserId;
        }

        public override int GetHashCode()
        {
            return (UserId + Name + AddingDate).GetHashCode();
        }
    }

    public class CsvDatabase : IDataBase
    {
        private readonly string usersPath;
        private readonly string plantsPath;
        private readonly string commonPlantsPath;

        public CsvDatabase(string usersPath, string plantsPath, string commonPlantsPath)
        {
            this.usersPath = usersPath;
            this.plantsPath = plantsPath;
            this.commonPlantsPath = commonPlantsPath;
        }
        
        public IEnumerable<User> GetUsers()
        {
            using (var stream = File.Open(usersPath, FileMode.Open))
            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Configuration.Delimiter = ";";
                return csvReader.GetRecords<User>().ToList();;
            }
        }

        public User GetUserById(long userId)
        {
            return (from user in GetUsers() where user.Id == userId select user).FirstOrDefault()!;
        }

        public void UpdateUser(User newUser)
        {
            var users = GetUsers().ToList();
            for (var i = 0; i < users.Count; i++)
            {
                if (!users[i].Equals(newUser)) 
                    continue;
                if (newUser.Name == null)
                    newUser.Name = users[i].Name;
                users[i] = newUser;
                break;
            }
            using var streamWriter = new StreamWriter(usersPath);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.Configuration.Delimiter = ";";
            csvWriter.WriteRecords(users);
        }

        public void UpdatePlant(Plant currentPlant)
        {
            var plants = GetPlants().ToList();
            for (var i = 0; i < plants.Count; i++)
            {
                if (!plants[i].Equals(currentPlant)) 
                    continue;
                if (currentPlant.ShouldBeDeleted is true)
                    plants.RemoveAt(i);
                else
                    plants[i] = currentPlant;
                break;
            }
            using var streamWriter = new StreamWriter(plantsPath);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.Configuration.Delimiter = ";";
            csvWriter.WriteRecords(plants);
        }

        public void DeletePlant(Plant currentPlant)
        {
            currentPlant.ShouldBeDeleted = true;
            UpdatePlant(currentPlant);
        }

        public void AddUser(User user)
        {
            AddUsers(new List<User> { user });
        }
        
        private void AddUsers(IEnumerable<User> users)
        {
            DoRecords(users, usersPath);
        }

        public void AddPlant(Plant plant)
        {
            AddPlants(new List<Plant> { plant });
        }
        
        private void AddPlants(IEnumerable<Plant> plants)
        {
            DoRecords(plants, plantsPath);
        }

        public IEnumerable<Plant> GetPlantsByUser(User user)
        {
            return GetPlants()
                .Where(plant => plant.UserId == user.Id)
                .ToList();
        }
        
        public IEnumerable<Plant> GetPlants()
        {
            using var reader = new StreamReader(plantsPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Configuration.Delimiter = ";";
            return csvReader
                .GetRecords<Plant>()
                .ToList();
        }
        
        private void DoRecords<T>(IEnumerable<T> records, string filePath)
        {
            if (File.Exists(filePath))
            {
                using var stream = File.Open(filePath, FileMode.Append);
                using var streamWriter = new StreamWriter(stream);
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                csvWriter.Configuration.HasHeaderRecord = false;
                csvWriter.Configuration.Delimiter = ";";
                csvWriter.WriteRecords<T>(records);
            }
            else
                throw new FileNotFoundException();
        }
    }
}