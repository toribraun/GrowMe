using System.Collections.Generic;

namespace Application.Replies
{
    public class ReplyOnGetPlantsToDelete : IReply
    {
        public long UserId { get; }
        public IEnumerable<string> PlantsName { get; }

        public ReplyOnGetPlantsToDelete(long userId, IEnumerable<string> plantsName)
        {
            UserId = userId;
            PlantsName = plantsName;
        }
    }
}