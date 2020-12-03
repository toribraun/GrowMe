using System;
using System.Collections.Generic;

namespace Application.Replies
{
    public class ReplyOnGetPlants : IReply
    {
        public long UserId { get; }
        public IEnumerable<string> PlantsName { get; }

        public ReplyOnGetPlants(long userId, IEnumerable<string> plantsName)
        {
            UserId = userId;
            PlantsName = plantsName;
        }
        
    }
}