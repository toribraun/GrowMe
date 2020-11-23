namespace UserInterface
{
    using System;
    using Application;
    using Domain;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class Program
    {
        private static App app = new App();
        private static ICommandExecutor executor = new CommandExecutor(app);
        private static KeyboardController keyboardController = new KeyboardController(app);
        private static TelegramBotClient client;
        private static string token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";

        public static void Main(string[] args)
        {
            var commands = new IUserCommand[]
                {
                    new Commands.StartCommand(),
                    new Commands.HelpCommand(),
                    new Commands.GetPlantsCommand(),
                    new Commands.AddPlantCommand(),
                    new Commands.DeletePlantCommand(),
                    new Commands.CancelCommand()
                };
            var commonStatus = new UserStatus[] { UserStatus.DefaultStatus, UserStatus.SendUserName };
            for (int i = 0; i < 5; i++)
            {
                executor.AddCommand(commands[i], commonStatus);
            }

            executor.AddCommand(commands[5], UserStatus.SendPlantName);
            executor.AddCommand(commands[5], UserStatus.SendPlantWateringInterval);
            executor.AddCommand(commands[5], UserStatus.DeletePlantByName);
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var answer = "Я пока не знаю, что с этим делать!";
            if (message?.Type == MessageType.Text)
            {
                answer = executor.ExecuteCommand(message);
                var messageText = message.Text;
                IReplyMarkup keyboard = keyboardController.GetKeyboard(message);
                SendAnswer(message.Chat, answer, keyboard);
            }
        }

        private static void SendAnswer(Chat chat, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(chat.Id, answer, replyMarkup: rm);
        }
    }
}