namespace UserInterface.Commands
{
    internal class DeletePlantCommand
    {
        private string[] names;

        public DeletePlantCommand()
        {
            this.names = new string[] { "/delete", "/deleteplant", "delete", "deleteplant", "удалить" };
        }

        public string[] Names => this.names;

        // public Answer Execute(Message message, App app)
        // {
        //     var plants = app
        //         .GetPlantsByUser(new Domain.User(message.Chat.Id, message.Chat.FirstName))
        //         .Split("\n");
        //     if (plants.Length == 0)
        //     {
        //         return new Answer("У тебя пока не записано растений!", message.Chat.Id);
        //     }
        //     else
        //     {
        //         app.ChangeUserStatus(message.Chat.Id, UserStatus.DeletePlantByName);
        //         return new Answer("Какое растение ты хочешь удалить?", message.Chat.Id, UserStatus.DeletePlantByName);
        //     }
        // }
    }
}
