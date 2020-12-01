namespace Application.Replies
{
    public class ReplyOnDeletePlants : IReply
    {
        public long UserId { get; }
        public string DeletedPlantName { get; }
    }
}