namespace UserInterface
{
    using Application;
    using Ninject;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var container = new StandardKernel();
            container.Bind<App>().ToConstant(app);
            container.Bind<ICommandExecutor>().To<CommandExecutor>();
            container.Bind<KeyboardController>().ToSelf();
            container.Bind<UI>().ToSelf();
            container.Get<UI>().Run();

            // var ui = new UI(
            //     app,
            //     new CommandExecutor(app),
            //     new KeyboardController(app));
            // ui.Run();

            // app.OnReplyRequest = ui.SendMessage;
            // addPlantCommand.OnAddPlantToUser += addPlantToUserHandler.AddPlantToUser;
            // addPlantToUserHandler.Execute
        }
    }
}