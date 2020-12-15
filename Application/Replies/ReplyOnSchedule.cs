namespace Application.Replies
{
    public class ReplyOnSchedule : IReply
    {
        public long UserId { get; }
        public string ScheduleMessage { get;  }

        public ReplyOnSchedule(long userId, string scheduleMessage)
        {
            UserId = userId;
            ScheduleMessage = scheduleMessage;
        }
    }
}