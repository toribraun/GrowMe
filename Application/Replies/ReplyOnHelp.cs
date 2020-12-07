namespace Application.Replies
{
    public class ReplyOnHelp : IReply
    {
        public long UserId { get; }
        public ReplyOnHelp(long userId)
        {
            UserId = userId;
        }
    }
}