using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    public class Schedule : Statistics
    {
        private IEnumerable<Plant> plants;
        public Schedule(IEnumerable<Plant> plants) : base(plants)
        {
            this.plants = plants;
        }

        public override string GetStatistics()
        {
            var plantsToWaterByDay = GetPlantsToWaterByDayDictionary();
            var result = "";
            foreach (var date in plantsToWaterByDay.Keys)
            {
                var currentPlants = plantsToWaterByDay[date]
                    .Aggregate("", (current, plant) => current + $"{plant}, ");

                result += $"{date.Day}.{date.Month}: {currentPlants.TrimEnd(',', ' ')}\n";
            }

            return result;
        }
        
        private Dictionary<DateTime, List<string>> GetPlantsToWaterByDayDictionary()
        {
            var plantsToWaterByDay = GetEmptyPlantsToWaterByDayDictionary();
            
            foreach (var currentPlant in plants.TakeWhile(currentPlant => currentPlant.Name != ""))
            {
                for (var nextDate = currentPlant.NextWateringTime.Date;
                    nextDate < DateTime.Today.Date.AddDays(8);
                    nextDate = nextDate.AddDays(currentPlant.WateringInterval))
                {
                    plantsToWaterByDay[nextDate].Add(currentPlant.Name);
                }
            }

            return plantsToWaterByDay;
        }
        
        private static Dictionary<DateTime, List<string>> GetEmptyPlantsToWaterByDayDictionary()
        {
            var plantsToWaterByDay = new Dictionary<DateTime, List<string>>();

            for (var i = 1; i < 8; i++)
            {
                plantsToWaterByDay.Add(DateTime.Today.Date.AddDays(i), new List<string>());
            }

            return plantsToWaterByDay;
        }
    }
}