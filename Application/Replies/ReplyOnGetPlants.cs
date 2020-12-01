using System.Collections.Generic;

namespace Application.Replies
{
    public class ReplyOnGetPlants : IReply
    {
        public long UserId { get; }
        public List<string> Plants { get; }
    }
}