namespace Application.Replies
{
    public class ReplyOnDeletedPlant : IReply
    {
        public long UserId { get; }
        public string DeletedPlantName { get; }
        public bool IsDeleted { get; }

        public ReplyOnDeletedPlant(long userId, string deletedPlantName, bool isDeleted)
        {
            UserId = userId;
            DeletedPlantName = deletedPlantName;
            IsDeleted = isDeleted;
        }
    }
}