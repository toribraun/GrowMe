namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class CancelCommand : IUserCommand
    {
        private string[] names;

        public CancelCommand()
        {
            this.names = new string[] { "/cancel", "cancel", "отмена" };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            app.Cancel(message.Chat.Id);
            return "Ну хорошо, ты снова в главном меню";
        }
    }
}
