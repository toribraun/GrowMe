using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Domain;

namespace Infrastructure
{
    public interface IUserRepository
    {
        User GetUser(long userId);
        void UpdateUser(User newUser);
        void AddNewUser(User newUser);
    }
    
    public interface IPlantRepository
    {
        IEnumerable<Plant> GetPlantsByUser(long userId);
        
        void UpdatePlant(Plant currentPlant);
        void DeletePlant(Plant currentPlant);
        IEnumerable<Plant> GetPlantsToWater();
        void AddNewPlant(Plant newPlant);
    }

    public interface IDatabaseTable<T>
    {
        IEnumerable<T> GetAllData();
        void WriteAllData(IEnumerable<T> newRecords);
        void AddNewRecord(T newRecord);
    }

    public class DatabaseCsvTable<T> : IDatabaseTable<T>
    {
        private string path;
        public string Header { get; }

        public DatabaseCsvTable(string path)
        {
            this.path = path;
            var properties = typeof(T).GetProperties().Select(p => p.Name);
            Header = string.Join(";", properties);
        }

        public IEnumerable<T> GetAllData()
        {
            using var stream = File.Open(path, FileMode.Open);
            using var reader = new StreamReader(stream);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Configuration.Delimiter = ";";
            return csvReader.GetRecords<T>().ToList();
        }

        public void WriteAllData(IEnumerable<T> newRecords)
        {
            using var streamWriter = new StreamWriter(path);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.Configuration.Delimiter = ";";
            csvWriter.WriteRecords(newRecords);
        }

        public void AddNewRecord(T newRecord)
        {
            using var stream = File.Open(path, FileMode.Append);
            using var streamWriter = new StreamWriter(stream);
            using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            csvWriter.Configuration.HasHeaderRecord = false;
            csvWriter.Configuration.Delimiter = ";";
            csvWriter.WriteRecords(new List<T> {newRecord});
        }
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseTable<User> userTable;

        public UserRepository(IDatabaseTable<User> userTable)
        {
            this.userTable = userTable;
        }
        
        public User GetUser(long userId)
        {
            return userTable.GetAllData()
                //.Select(a => new User(long.Parse(a[0]), a[1]))
                .FirstOrDefault(u => u.Id == userId);
        }

        public void UpdateUser(User newUser)
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

        public void AddNewUser(User user)
        {
            userTable.AddNewRecord(user);
        }
    }

    public class PlantRepository : IPlantRepository
    {
        private IDatabaseTable<Plant> plantTable;
        
        public PlantRepository(IDatabaseTable<Plant> plantTable)
        {
            this.plantTable = plantTable;
        }
        public IEnumerable<Plant> GetPlantsByUser(long userId)
        {
            return plantTable.GetAllData()
                .Where(plant => plant.UserId == userId)
                .ToList();
        }

        public void UpdatePlant(Plant currentPlant)
        {
            var plants = plantTable.GetAllData().ToList();
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
            plantTable.WriteAllData(plants);
        }

        public void DeletePlant(Plant currentPlant)
        {
            currentPlant.ShouldBeDeleted = true;
            UpdatePlant(currentPlant);
        }

        public IEnumerable<Plant> GetPlantsToWater()
        {
            var now = DateTime.Now;
            return plantTable.GetAllData()
                .Where(plant => plant.NextWateringTime <= now)
                .Select(plant =>
                {
                    plant.UpdateNextWateringTime();
                    UpdatePlant(plant);
                    return plant;
                });
        }

        public void AddNewPlant(Plant newPlant)
        {
            plantTable.AddNewRecord(newPlant);
        }
    }
}