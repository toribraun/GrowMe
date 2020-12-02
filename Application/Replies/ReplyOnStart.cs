namespace Application.Replies
{
    public class ReplyOnStart : IReply
    {
        public long UserId { get; }
        public string UserName { get; }
        public bool IsAdded { get; }

        public ReplyOnStart(long userId, string userName, bool isAdded)
        {
            UserId = userId;
            UserName = userName;
            IsAdded = isAdded;
        }
    }
}