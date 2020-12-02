namespace Application.Replies
{
    public class ReplyOnCancel : IReply
    {
        public long UserId { get; }

        public ReplyOnCancel(long userId)
        {
            UserId = userId;
        }
    }
}