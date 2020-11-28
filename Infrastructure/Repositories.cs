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
        UserRecord GetUser(long userId);
        void UpdateUser(UserRecord newUser);
        void AddNewUser(UserRecord newUser);
    }
    
    public interface IPlantRepository
    {
        IEnumerable<PlantRecord> GetPlantsByUser(long userId);
        
        void UpdatePlant(PlantRecord currentPlant);
        void DeletePlant(PlantRecord currentPlant);
        IEnumerable<PlantRecord> GetPlantsToWater();
        void AddNewPlant(PlantRecord newPlant);
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

    public class PlantRepository : IPlantRepository
    {
        private IDatabaseTable<PlantRecord> plantTable;
        
        public PlantRepository(IDatabaseTable<PlantRecord> plantTable)
        {
            this.plantTable = plantTable;
        }
        public IEnumerable<PlantRecord> GetPlantsByUser(long userId)
        {
            return plantTable.GetAllData()
                .Where(plant => plant.UserId == userId)
                .ToList();
        }

        public void UpdatePlant(PlantRecord currentPlant)
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

        public void DeletePlant(PlantRecord currentPlant)
        {
            currentPlant.ShouldBeDeleted = true;
            UpdatePlant(currentPlant);
        }

        public IEnumerable<PlantRecord> GetPlantsToWater()
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

        public void AddNewPlant(PlantRecord newPlant)
        {
            plantTable.AddNewRecord(newPlant);
        }
    }
}