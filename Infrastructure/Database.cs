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
        void AddOrUpdateUser(User user);
        IEnumerable<Plant> GetPlantsUser(User user);
    }
    
    public class User
    {
        [Name("id")]
        public int Id { get; }

        public User(int id)
        {
            Id = id;
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
    }

    public class CsvDatabase : IDataBase
    {
        public IEnumerable<User> GetUsers()
        {
            var pathCsvFile = "users.csv";
            using (var reader = new StreamReader(pathCsvFile))
            using (CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Configuration.Delimiter = ";";
                return csvReader.GetRecords<User>();
            }
        }
        public void AddOrUpdateUser(User user)
        {
            
        }

        public IEnumerable<Plant> GetPlantsUser(User user)
        {
            var pathCsvFile = "users_plants.csv";
            using (var reader = new StreamReader(pathCsvFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Configuration.Delimiter = ";";
                return csvReader
                    .GetRecords<Plant>()
                    .Where(plant => plant.UserId == user.Id)
                    .ToList();
            }
        }
    }

}