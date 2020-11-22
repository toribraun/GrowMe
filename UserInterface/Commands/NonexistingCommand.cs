namespace UserInterface.Commands
{
    using System;
    using Application;
    using Infrastructure;
    using Telegram.Bot.Types;

    internal class NonexistingCommand : IUserCommand
    {
        private string[] names;

        public NonexistingCommand()
        {
            this.names = new string[] { string.Empty };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            var status = app.GetUserStatus(message.Chat.Id);
            if (status == UserStatus.SendPlantName)
            {
                if (app.SetNewPlantName(message.Chat.Id, message.Text))
                {
                    return "У тебя уже есть растение с таким именем! Придумай другое.";
                }
                else
                {
                    return "Как часто нужно поливать твоё растение? Укажи интервал в сутках.\n" +
                             "Например, если твоё растение нужно поливать каждые три дня, напиши: 3.";
                }
            }
            else if (status == UserStatus.SendPlantWateringInterval)
            {
                var answer = "Пожалуйста, введи целое положительное число";
                try
                {
                    if (int.TryParse(message.Text, out var interval))
                    {
                        app.AddNewPlantFromActivePlantWithWateringInterval(message.Chat.Id, interval);
                        answer = "Поздравляю, твоё растение добавлено!";
                    }
                }
                catch (ArgumentException) { }
                return answer;
            }
            else
            {
                return "Я не понимаю тебя. Если нужна справка, введи /help!";
            }
        }
    }
}
