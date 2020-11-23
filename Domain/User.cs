namespace Domain
{
    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public UserStatus Status { get; set; }
        public string ActivePlantName { get; set; }
        
        public User() {}
        public User(long id)
        {
            Id = id;
        }

        public User(long id, string name)
        {
            Id = id;
            Name = name;
            Status = UserStatus.DefaultStatus;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Id == ((User)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
    
    public enum UserStatus
    {
        DefaultStatus,
        SendUserName,
        SendPlantName,
        SendPlantWateringInterval,
        DeletePlantByName
    }
}