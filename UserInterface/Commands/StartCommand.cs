namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class StartCommand : IUserCommand
    {
        private string[] names;

        public StartCommand()
        {
            this.names = new string[] { "/start" };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            var chat = message.Chat;
            var user = new Domain.User(chat.Id, chat.FirstName);
            if (app.AddUser(user))
            {
                return $"Привет, {chat.FirstName}!\n" +
                    $"Я - бот, который будет помогать тебе в уходе за растениями.\n" +
                    $"Введи /help для справки.";
            }

            return $"Снова здравствуй, {chat.FirstName}! Если нужна справка - введи /help";
        }
    }
}
