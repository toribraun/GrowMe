namespace Application.Replies
{
    public class ReplyOnSetPlantName : IReply
    {
        public long UserId { get; }

        public ReplyOnSetPlantName(long userId)
        {
            UserId = userId;
        }
    }
}