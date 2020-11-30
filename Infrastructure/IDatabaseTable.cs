using System.Collections.Generic;

namespace Infrastructure
{
    public interface IDatabaseTable<T>
    {
        IEnumerable<T> GetAllData();
        void WriteAllData(IEnumerable<T> newRecords);
        void AddNewRecord(T newRecord);
    }
}