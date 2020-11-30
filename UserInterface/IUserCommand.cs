namespace UserInterface
{
    using Application;
    using Telegram.Bot.Types;

    public interface IUserCommand
    {
        public string[] Names { get; }

        public Answer Execute(Message message, App app);
    }
}
