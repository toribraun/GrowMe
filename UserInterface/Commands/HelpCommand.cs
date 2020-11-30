namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class HelpCommand : IUserCommand
    {
        private string[] names;

        public HelpCommand()
        {
            this.names = new string[] { "/help", "help", "справка" };
        }

        public string[] Names => this.names;

        public Answer Execute(Message message, App app)
        {
            return new Answer(
                "Если ты видишь это сообщение, и ты не один из моих создателей, можешь пнуть их, чтобы они сделали наконец справку:D",
                message.Chat.Id);
        }
    }
}
