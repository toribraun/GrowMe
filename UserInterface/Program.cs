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
            var executor = container.Get<CommandExecutor>();
            var ui = container.Get<UI>();
            var app = container.Get<App>();
            app.SendNotification += (sender, args) => ui.SendNotification(args.UserId, args.PlantName);
            app.OnReply += (sender, reply) => ui.BuildMessageToUser(reply);

            executor.OnStart += (sender, eventArgsStart) => app.StartEvent(eventArgsStart.UserId, eventArgsStart.UserName);
            executor.OnCancel += (sender, userId) => app.Cancel(userId);
            executor.OnGetPlantsToDelete += (sender, userId) => app.GetPlantsToDeleteEvent(userId);
            executor.OnGetPlants += (sender, userId) => app.GetPlantsByUserEvent(userId);
            executor.OnAddPlant += (sender, userId) => app.AddPlantByUserEvent(userId);
            executor.OnNonexistingCommand += (sender, commandArgs) => app.HandleNonexistingCommand(commandArgs.UserId, commandArgs.Message);
            executor.OnCheckUserExist += (sender, checkUserArgs) => app.CheckUserExistEvent(checkUserArgs.UserId, checkUserArgs.UserName);
            executor.OnHelp += (sender, userId) => app.GetHelp(userId);

            ui.Run();
        }
    }
}