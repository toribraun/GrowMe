using System;

namespace Domain
{
    public class Plant
    {
        public string Name { get; set; }
        public long UserId { get; set; }

        private int wateringInterval;
        public int WateringInterval {
            get => wateringInterval;
            set
            {
                // if (value <= 0)
                //     throw new ArgumentException();
                wateringInterval = value;
            } }
        public DateTime NextWateringTime { get; set; }
        public bool WateringStatus { get; set; }
        public DateTime AddingDate { get; set; }
        public bool ShouldBeDeleted { get; set; }
        
        public Plant() {}

        public Plant(string name, long userId)
        {
            Name = name;
            UserId = userId;
        }

        public Plant(string name, long userId, int wateringInterval)
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
            return Name == ((Plant) obj).Name && UserId == ((Plant) obj).UserId;
        }

        public override int GetHashCode()
        {
            return (UserId + Name + AddingDate).GetHashCode();
        }

        public void UpdateNextWateringTime()
        {
            NextWateringTime = DateTime.Now.AddDays(wateringInterval);
        }
    }
}