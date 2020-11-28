namespace UserInterface.Commands
{
    using System;
    using Application;
    using Domain;
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
                return this.NamePlant(message, app);
            }
            else if (status == UserStatus.SendPlantWateringInterval)
            {
                return this.SetWateringInterval(message, app);
            }
            else if (app.GetUserStatus(message.Chat.Id) == UserStatus.DeletePlantByName)
            {
                return this.DeletePlant(message, app);
            }
            else
            {
                return "Я не понимаю тебя. Если нужна справка, введи /help!";
            }
        }

        private string DeletePlant(Message message, App app)
        {
            if (app.DeletePlant(message.Chat.Id, message.Text))
            {
                return $"Растение {message.Text} удалено!";
            }
            else
            {
                return "У тебя нет растения с таким именем!";
            }
        }

        private string SetWateringInterval(Message message, App app)
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

        private string NamePlant(Message message, App app)
        {
            var input = message.Text.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            if (input.Length > 1)
            {
                return "Я не могу записать такое имя, напиши в одну строку, пожалуйста!";
            }

            var name = input[0];
            if (name.Length > 25)
            {
                return "Это длинное имя, придумай покороче, пожалуйста!";
            }
            else if (!app.SetNewPlantName(message.Chat.Id, name))
            {
                return "У тебя уже есть растение с таким именем! Придумай другое.";
            }
            else
            {
                return "Как часто нужно поливать твоё растение? Укажи интервал в сутках.\n" +
                         "Например, если твоё растение нужно поливать каждые три дня, напиши: 3.";
            }
        }
    }
}
