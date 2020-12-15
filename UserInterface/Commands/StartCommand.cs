namespace UserInterface.Commands
{
    internal class StartCommand
    {
        private string[] names;

        public StartCommand()
        {
            this.names = new string[] { "/start" };
        }

        public string[] Names => this.names;

        // public Answer Execute(Message message, App app)
        // {
        //     var chat = message.Chat;
        //     if (app.AddUser(chat.Id, chat.FirstName))
        //     {
        //         return new Answer($"Привет, {chat.FirstName}!\n" +
        //             $"Я - бот, который будет помогать тебе в уходе за растениями.\n" +
        //             $"Введи /help для справки.", message.Chat.Id);
        //     }
        //
        //     return new Answer($"Снова здравствуй, {chat.FirstName}! Если нужна справка - введи /help", message.Chat.Id);
        // }
    }
}
