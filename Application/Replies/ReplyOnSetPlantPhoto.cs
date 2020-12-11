namespace Application.Replies
{
    public class ReplyOnSetPlantPhoto : IReply
    {
        public long UserId { get; }
        public string PlantName { get; }
        public bool IsAdded { get; }

        public ReplyOnSetPlantPhoto(long userId, string plantName, bool isAdded)
        {
            UserId = userId;
            PlantName = plantName;
            IsAdded = isAdded;
        }
    }
}