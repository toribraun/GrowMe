using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure
{
    public class PlantRepository : IPlantRepository
    {
        private IDatabaseTable<PlantRecord> plantTable;
        
        public PlantRepository(IDatabaseTable<PlantRecord> plantTable)
        {
            this.plantTable = plantTable;
        }
        public IEnumerable<PlantRecord> GetPlantsByUser(long userId)
        {
            return plantTable.GetAllData()
                .Where(plant => plant.UserId == userId)
                .ToList();
        }

        public void UpdatePlant(PlantRecord currentPlant)
        {
            var plants = plantTable.GetAllData().ToList();
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
            plantTable.WriteAllData(plants);
        }

        public void DeletePlant(PlantRecord currentPlant)
        {
            currentPlant.ShouldBeDeleted = true;
            UpdatePlant(currentPlant);
        }

        public IEnumerable<PlantRecord> GetPlantsToWater()
        {
            var now = DateTime.Now;
            return plantTable.GetAllData()
                .Where(plant => plant.NextWateringTime <= now)
                .Select(plant =>
                {
                    plant.UpdateNextWateringTime();
                    UpdatePlant(plant);
                    return plant;
                });
        }

        public void AddNewPlant(PlantRecord newPlant)
        {
            plantTable.AddNewRecord(newPlant);
        }
    }
}