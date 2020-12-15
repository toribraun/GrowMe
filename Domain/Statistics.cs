using System.Collections.Generic;

namespace Domain
{
    public abstract class Statistics
    {
        public IEnumerable<Plant> Plants { get; }

        public Statistics(IEnumerable<Plant> plants)
        {
            Plants = plants;
        }

        public abstract string GetStatistics();
    }
}