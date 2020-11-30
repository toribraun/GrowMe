using Domain;

namespace UserInterface
{
    public class Answer
    {
        public string AnswerText { get; }

        public UserStatus Status { get; }

        public long UserId { get; }

        public Answer(string answer, long userId, UserStatus status = UserStatus.DefaultStatus)
        {
            UserId = userId;
            Status = status;
            AnswerText = answer;
        }
    }
}