using Infrastructure;

namespace UserInterface
{
    using Application;
    using Ninject;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var container = new StandardKernel();

            container.Bind<IUserRepository>()
                .ToConstant(new UserRepository(new DatabaseCsvTable<UserRecord>("users.csv")));
            container.Bind<IPlantRepository>()
                .ToConstant(new PlantRepository(new DatabaseCsvTable<PlantRecord>("users_plants.csv")));
            container.Bind<App>().ToSelf();

            container.Bind<ICommandExecutor>().To<CommandExecutor>();
            container.Bind<KeyboardController>().ToSelf();
            container.Bind<UI>().ToSelf();
            container.Get<UI>().Run();

            // app.OnReplyRequest = ui.SendMessage;
            // addPlantCommand.OnAddPlantToUser += addPlantToUserHandler.AddPlantToUser;
            // addPlantToUserHandler.Execute
        }
    }
}