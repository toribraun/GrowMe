using System;
using System.Linq;

namespace Infrastructure
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new CsvDatabase();
            var plants = db.GetPlantsUser(new User(1));
            Console.WriteLine(plants.First().Name);
        }
    }
}