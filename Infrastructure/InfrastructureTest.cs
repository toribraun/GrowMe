using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Infrastructure
{
    [TestFixture]
    public class InfrastructureTest
    {
        private CsvDatabase csvDB;

        [SetUp]
        public void SetUp()
        {
            using (var fsUsers = new FileStream("test_users.csv", FileMode.Create))
            {
                var array = System.Text.Encoding.Default.GetBytes("id;name;status\n");
                fsUsers.Write(array, 0, array.Length);
            }
            using (var fsPlants = new FileStream("test_users_plants.csv", FileMode.Create))
            {
                var array = System.Text.Encoding.Default.GetBytes("name;userId;wateringInterval;nextWateringTime;wateringStatus;addingDate\n");
                fsPlants.Write(array, 0, array.Length);
            }

            csvDB = new CsvDatabase("test_users.csv", "test_users_plants.csv", "test_common_plants.csv");
        }
        
        [TearDown]
        public void TearDown()
        {
            File.Delete("test_users.csv");
            File.Delete("test_users_plants.csv");
            File.Delete("test_common_plants.csv");
        }
        
        [Test]
        public void AddAndGetUsers()
        {
            var usersNew = new List<User>
            {
                new User(1, "Alice"),
                new User(2, "Bob")
            };
            
            csvDB.AddUsers(usersNew);
            var addedUsers = csvDB.GetUsers();
            CollectionAssert.AreEquivalent(usersNew, addedUsers);
        }
        
        [Test]
        public void UpdateUserStatus()
        {
            var newUser = new User(33, "Eva");
            csvDB.AddUsers(new List<User> {newUser});
            csvDB.UpdateUserStatus(newUser, UserStatus.SendUserName);
            var addedUser = csvDB.GetUsers().First(u => u.Id == 33);
            Assert.AreEqual(UserStatus.SendUserName, addedUser.Status);
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
            
            csvDB.AddPlants(plantsNew);
            var addedPlantsByUser1 = csvDB.GetPlantsByUser(new User(1, "Alice")).ToList();
            Assert.AreEqual(1, addedPlantsByUser1.Count());
            Assert.AreEqual(plantsNew.First(), addedPlantsByUser1.First());
        }
    }
}