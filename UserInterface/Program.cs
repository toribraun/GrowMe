namespace UserInterface
{
    using Application;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var ui = new UI(
                app,
                new CommandExecutor(app),
                new KeyboardController(app),
                "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98");
            ui.Run();
            // app.OnReplyRequest = ui.SendMessage;
            // addPlantCommand.OnAddPlantToUser += addPlantToUserHandler.AddPlantToUser;
            // addPlantToUserHandler.Execute
        }
    }
}