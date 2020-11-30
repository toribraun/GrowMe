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

        public Answer Execute(Message message, App app)
        {
            app.Cancel(message.Chat.Id);
            return new Answer("Ну хорошо, ты снова в главном меню", message.Chat.Id);
        }
    }
}
