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
                .ToConstant(new UserRepository(new DatabaseCsvTable<UserRecord>("users.csv")))
                .InSingletonScope();
            container.Bind<IPlantRepository>()
                .ToConstant(new PlantRepository(new DatabaseCsvTable<PlantRecord>("users_plants.csv")))
                .InSingletonScope();
            container.Bind<App>().ToSelf().InSingletonScope();

            container.Bind<ICommandExecutor>().To<CommandExecutor>().InSingletonScope();
            container.Bind<KeyboardController>().ToSelf().InSingletonScope();
            container.Bind<UI>().ToSelf().InSingletonScope();
            var ui = container.Get<UI>();
            var app = container.Get<App>();
            var executor = container.Get<CommandExecutor>();
            app.SendNotification += (sender, args) => ui.SendNotification(args.UserId, args.PlantName);
            app.OnReply += (sender, reply) => ui.BuildMessageToUser(reply);

            // executor.OnStart += app.StartEvent;
            // executor.OnCancel += app.Cancel;
            // executor.OnGetPlantsToDelete += app.GetPlantsToDeleteEvent;
            // executor.OnGetPlants += app.GetPlantsByUserEvent;
            // executor.OnAddPlant += app.AddPlantByUserEvent;
            // executor.OnNonexistingCommand += app.HandleNonexistingCommand;
            // executor.OnCheckUserExist += app.CheckUserExistEvent;
            // executor.OnHelp += app.GetHelp;

            ui.Run();
        }
    }
}