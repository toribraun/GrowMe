#nullable enable
using System;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper.Configuration.Attributes;
using Domain;

namespace Infrastructure
{
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