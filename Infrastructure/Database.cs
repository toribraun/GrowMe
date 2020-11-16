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
        void AddUsers(IEnumerable<User> users);
        IEnumerable<Plant> GetPlantsUser(User user);
    }
    
    public class User
    {
        [Name("id")]
        public int Id { get; set; }
        [Name("name")]
        public string Name { get; set; }
        
        public User() {}

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Id == ((User)obj).Id && Name == ((User)obj).Name;
        }

        public override int GetHashCode()
        {
            return (Id + Name).GetHashCode();
        }
    }

    public class Plant
    {
        [Name("name")]
        public string Name { get; set; }
        [Name("userId")]
        public int UserId { get; set; }
        [Name("nextWateringTime")]
        public DateTime NextWateringTime { get; set; }
        [Name("wateringStatus")]
        public bool WateringStatus { get; set; }
        [Name("addingDate")]
        public DateTime AddingDate { get; set; }
        
        public Plant() {}
        
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Name == ((Plant)obj).Name && UserId == ((Plant)obj).UserId && AddingDate == ((Plant)obj).AddingDate;
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
        public void AddUsers(IEnumerable<User> users)
        {
            DoRecords(users, usersPath);
        }
        
        public void AddPlants(IEnumerable<Plant> plants)
        {
            DoRecords(plants, plantsPath);
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
        
        public IEnumerable<Plant> GetPlantsUser(User user)
        {
            using var reader = new StreamReader(plantsPath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            csvReader.Configuration.Delimiter = ";";
            return csvReader
                .GetRecords<Plant>()
                .Where(plant => plant.UserId == user.Id)
                .ToList();
        }
    }

}