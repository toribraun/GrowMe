namespace Application.Replies
{
    public class ReplyOnSetWateringInterval : IReply
    {
        public long UserId { get; }

        public ReplyOnSetWateringInterval(long userId)
        {
            UserId = userId;
        }
    }
}