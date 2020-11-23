using Domain;

namespace UserInterface.Commands
{
    using Application;
    using Infrastructure;
    using Telegram.Bot.Types;

    internal class DeletePlantCommand : IUserCommand
    {
        private string[] names;

        public DeletePlantCommand()
        {
            this.names = new string[] { "/delete", "/deleteplant", "delete", "deleteplant", "удалить" };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            var plants = app
                .GetPlantsByUser(new Domain.User(message.Chat.Id, message.Chat.FirstName))
                .Split("\n");
            if (plants[0] == string.Empty)
            {
                return "У тебя пока не записано растений!";
            }
            else
            {
                app.ChangeUserStatus(message.Chat.Id, UserStatus.DeletePlantByName);
                return "Какое растение ты хочешь удалить?";
            }
        }
    }
}
