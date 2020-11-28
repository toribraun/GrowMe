namespace Infrastructure
{
    public interface IUserRecord
    {
        
    }
    
    public class UserRecord
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public UserStatusRecord Status { get; set; }
        public string ActivePlantName { get; set; }
        
        public UserRecord() {}
        public UserRecord(long id)
        {
            Id = id;
        }

        public UserRecord(long id, string name)
        {
            Id = id;
            Name = name;
            Status = UserStatusRecord.DefaultStatus;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Id == ((UserRecord)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
    
    public enum UserStatusRecord
    {
        DefaultStatus,
        SendUserName,
        SendPlantName,
        SendPlantWateringInterval,
        DeletePlantByName
    }
}