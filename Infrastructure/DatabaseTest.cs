using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Domain;
using NUnit.Framework;

namespace Infrastructure
{
    [TestFixture]
    public class DatabaseTest
    {
        private IUserRepository userRepository;
        private IPlantRepository plantRepository;

        [SetUp]
        public void SetUp()
        {
            var userTable = new DatabaseCsvTable<User>("test_users.csv");
            var plantTable = new DatabaseCsvTable<Plant>("test_users_plants.csv");
            using (var fsUsers = new FileStream("test_users.csv", FileMode.Create))
            {
                var array = System.Text.Encoding.Default.GetBytes(userTable.Header + "\n");
                fsUsers.Write(array, 0, array.Length);
            }
            using (var fsPlants = new FileStream("test_users_plants.csv", FileMode.Create))
            {
                var array = System.Text.Encoding.Default.GetBytes(plantTable.Header +"\n");
                fsPlants.Write(array, 0, array.Length);
            }
            userRepository = new UserRepository(userTable);
            plantRepository = new PlantRepository(plantTable);
        }
        
        [TearDown]
        public void TearDown()
        {
            File.Delete("test_users.csv");
            File.Delete("test_users_plants.csv");
        }
        
        [Test]
        public void AddAndGetUsers()
        {
            var usersNew = new List<User>
            {
                new User(1, "Alice"),
                new User(2, "Bob")
            };
            foreach (var user in usersNew)
            {
                userRepository.AddNewUser(user);
            }
            Assert.AreEqual(usersNew[0], userRepository.GetUser(usersNew[0].Id));
            Assert.AreEqual(usersNew[1], userRepository.GetUser(usersNew[1].Id));
        }
        
        [Test]
        public void UpdateAndGetUserStatus()
        {
            var newUser = new User(33, "Eva") {Status = UserStatus.SendUserName};
            userRepository.AddNewUser(newUser);
            userRepository.UpdateUser(newUser);
            //var addedUser = csvDB.GetUsers().First(u => u.Id == 33);
            Assert.AreEqual(UserStatus.SendUserName, userRepository.GetUser(newUser.Id).Status);
        }
        
        [Test]
        public void AddAndGetPlantsByUsers()
        {
            var plantsNew = new List<Plant>()
            {
                new Plant {Name = "tulpan", UserId = 1, WateringInterval = 3, AddingDate = DateTime.Parse("11.11.2020"), 
                    NextWateringTime = DateTime.Parse("11.11.2020 11:00:00"), WateringStatus = false},
                new Plant {Name = "cactus", UserId = 2, WateringInterval = 7, AddingDate = DateTime.Parse("11.11.2020"), 
                    NextWateringTime = DateTime.Parse("11.11.2020 11:00:00"), WateringStatus = false}
            };
            foreach (var plant in plantsNew)
            {
                plantRepository.AddNewPlant(plant);
            }
            var addedPlantsByUser1 = plantRepository.GetPlantsByUser(1).ToList();
            Assert.AreEqual(1, addedPlantsByUser1.Count());
            Assert.AreEqual(plantsNew.First(), addedPlantsByUser1.First());
        }
    }
}