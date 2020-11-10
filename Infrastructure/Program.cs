using System;
using System.Collections.Generic;

using System.Linq;

namespace Infrastructure
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"C:\Users\Anzhelika\Desktop\GrowMe\Infrastructure\users.csv";
            var users = new List<User>
            {
                new User { Id = 2, Name = "Abc" },
                new User { Id = 1, Name = "Cba" }
            };
            var usersBase = new Base(path);
            usersBase.DoRecords(users);
            foreach (var user in usersBase.GetRecords<User>())
            {
                Console.WriteLine($"Id: {user.Id.ToString()}, Name: {user.Name}");    
            }
            var db = new CsvDatabase();
            var plants = db.GetPlantsUser(new User(1));
            Console.WriteLine(plants.First().Name);
        }
    }
}