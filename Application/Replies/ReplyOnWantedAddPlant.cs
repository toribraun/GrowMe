namespace Application.Replies
{
    public class ReplyOnWantedAddPlant : IReply
    {
        public long UserId { get; }
        public bool TriedInvalidName { get; }

        public ReplyOnWantedAddPlant(long userId, bool triedInvalidName = false)
        {
            UserId = userId;
            TriedInvalidName = triedInvalidName;
        }
    }
}