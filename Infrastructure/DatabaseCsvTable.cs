using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Infrastructure
{
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
}