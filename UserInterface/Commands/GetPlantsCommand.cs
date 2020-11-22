namespace UserInterface.Commands
{
    using Application;
    using Telegram.Bot.Types;

    internal class GetPlantsCommand : IUserCommand
    {
        private string[] names;

        public GetPlantsCommand()
        {
            this.names = new string[] { "/getplants", "мои растения", "getplants", "покажи растения", "покажи мне растения" };
        }

        public string[] Names => this.names;

        public string Execute(Message message, App app)
        {
            var user = app.GetUserById(message.Chat.Id);
            var plants = app.GetPlantsByUser(user);
            if (plants.Length == 0)
            {
                return "У тебя пока не записано растений!";
            }
            else
            {
                return plants;
            }
        }
    }
}
