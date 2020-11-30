using System.Collections.Generic;

namespace Infrastructure
{
    public interface IPlantRepository
    {
        IEnumerable<PlantRecord> GetPlantsByUser(long userId);
        void UpdatePlant(PlantRecord currentPlant);
        void DeletePlant(PlantRecord currentPlant);
        IEnumerable<PlantRecord> GetPlantsToWater();
        void AddNewPlant(PlantRecord newPlant);
    }
}