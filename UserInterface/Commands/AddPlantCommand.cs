using System;
using Domain;

namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class AddPlantCommand : IUserCommand
    {
        private string[] names;

        public event Action<long, UserStatus> OnAddPlant;

        public AddPlantCommand()
        {
            this.names = new string[] { "/add", "/addplant", "add", "addplant", "добавить", "/newplant" };
        }

        public string[] Names => this.names;

        public Answer Execute(Message message, App app)
        {
            app.ChangeUserStatus(message.Chat.Id, UserStatus.SendPlantName);
            return new Answer("Как назовём твоё растение?", message.Chat.Id, UserStatus.SendPlantName);
        }
    }
}
