namespace Application.Replies
{
    public class ReplyOnWantedAddPlant : IReply
    {
        public long UserId { get; }

        public ReplyOnWantedAddPlant(long userId)
        {
            UserId = userId;
        }
    }
}