namespace UserInterface
{
    using Application;
    using Telegram.Bot.Types;

    public interface IUserCommand
    {
        public string[] Names { get; }

        public string Execute(Message message, App app);
    }
}
