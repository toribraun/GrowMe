using Domain;

namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class AddPlantCommand : IUserCommand
    {
        private string[] names;

        public AddPlantCommand()
        {
            this.names = new string[] { "/add", "/addplant", "add", "addplant", "добавить", "/newplant" };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            app.ChangeUserStatus(message.Chat.Id, UserStatus.SendPlantName);
            return "Как назовём твоё растение?";
        }
    }
}
