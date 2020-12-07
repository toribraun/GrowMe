namespace Application.Replies
{
    public class ReplyOnNotCommand : IReply
    {
        public long UserId { get; }
        public ReplyOnNotCommand(long userId)
        {
            UserId = userId;
        }
    }
}