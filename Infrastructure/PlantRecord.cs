using System;
using Domain;

namespace Infrastructure
{
    public class PlantRecord
    {
        public string Name { get; set; }
        public long UserId { get; set; }

        public int WateringInterval { get; set; }
        public DateTime NextWateringTime { get; set; }
        public bool WateringStatus { get; set; }
        public DateTime AddingDate { get; set; }
        public bool ShouldBeDeleted { get; set; }
        
        public PlantRecord() {}

        public PlantRecord(string name, long userId, int wateringInterval)
        {
            Name = name;
            UserId = userId;
            WateringInterval = wateringInterval;
            AddingDate = DateTime.Today;
            NextWateringTime = DateTime.Now.AddDays(WateringInterval);
        }
        
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Name == ((PlantRecord) obj).Name && UserId == ((PlantRecord) obj).UserId;
        }

        public override int GetHashCode()
        {
            return (UserId + Name + AddingDate).GetHashCode();
        }
        
        public void UpdateNextWateringTime()
        {
            NextWateringTime = DateTime.Now.AddDays(WateringInterval);
        }
    }
}