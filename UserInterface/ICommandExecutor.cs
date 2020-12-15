namespace UserInterface
{
    using Telegram.Bot.Types;

    public interface ICommandExecutor
    {
        public void ExecutePhoto(Message message);

        public void ExecuteCommandMessage(Message message);
    }
}
