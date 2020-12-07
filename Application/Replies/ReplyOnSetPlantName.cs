namespace Application.Replies
{
    public class ReplyOnSetPlantName : IReply
    {
        public long UserId { get; }
        public bool TriedInvalidInterval { get; }

        public ReplyOnSetPlantName(long userId, bool triedInvalidInterval = false)
        {
            UserId = userId;
            TriedInvalidInterval = triedInvalidInterval;
        }
    }
}