using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using System.IO;
using System.Linq;

namespace Infrastructure
{
    public interface IDataBase
    {
        // void AddOrUpdateUser(User user);
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Base
    {
        public string Path { get; }

        public Base(string path)
        {
            Path = path;
        }

        public void DoRecords<T>(IEnumerable<T> records)
        {
            if (File.Exists(Path))
            {
                using var stream = File.Open(Path, FileMode.Append);
                using var streamWriter = new StreamWriter(stream);
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                csvWriter.Configuration.HasHeaderRecord = false;
                csvWriter.WriteRecords(records);
            }
            else
            {
                using var stream = File.Create(Path);
                using var streamWriter = new StreamWriter(stream);
                using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
                csvWriter.WriteRecords(records);
            }
        }

        public List<T> GetRecords<T>()
        {
            using var reader = new StreamReader(Path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<T>().ToList();
        }
    }
}