namespace UserInterface.Commands
{
    internal class HelpCommand
    {
        private string[] names;

        public HelpCommand()
        {
            this.names = new string[] { "/help", "help", "справка" };
        }

        public string[] Names => this.names;

        // public Answer Execute(Message message, App app)
        // {
        //     return new Answer(
        //         "/plants — посмотреть список твоих растений\n" +
        //         "/add — добавить растение\n" +
        //         "/delete — удалить растение\n" +
        //         "Возможно, я пойму и твою фразу на простом человеческом русском!",
        //         message.Chat.Id);
        // }
    }
}
