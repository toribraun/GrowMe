namespace UserInterface
{
    using System;
    using Application;
    using Infrastructure;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class Program
    {
        private static App app = new App();
        private static ICommandExecutor executor = new CommandExecutor(app);
        private static TelegramBotClient client;
        private static string token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";
        private static IReplyMarkup mainMenuKeyboard = new ReplyKeyboardMarkup(
            new[]
        {
            new KeyboardButton("Мои растения"),
            new KeyboardButton("Добавить растение")
        }, true);

        private static IReplyMarkup cancelKeyboard = new ReplyKeyboardMarkup(
            new[]
        {
            new KeyboardButton("Отмена")
        }, true);

        public static void Main(string[] args)
        {
            var commands = new IUserCommand[]
                {
                    new Commands.StartCommand(),
                    new Commands.HelpCommand(),
                    new Commands.GetPlantsCommand(),
                    new Commands.AddPlantCommand(),
                    new Commands.CancelCommand()
                };
            var commonStatus = new UserStatus[] { UserStatus.DefaultStatus, UserStatus.SendUserName };
            for (int i = 0; i < 4; i++)
            {
                executor.AddCommand(commands[i], commonStatus);
            }

            executor.AddCommand(commands[4], UserStatus.SendPlantName);
            executor.AddCommand(commands[4], UserStatus.SendPlantWateringInterval);
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
            ReplyKeyboardMarkup keyboard;
            if (message?.Type == MessageType.Text)
            {
                answer = executor.ExecuteCommand(message);

                if (app.GetUserStatus(message.Chat.Id) == UserStatus.DefaultStatus)
                {
                    keyboard = (ReplyKeyboardMarkup)mainMenuKeyboard;
                }
                else
                {
                    keyboard = (ReplyKeyboardMarkup)cancelKeyboard;
                }

                if (keyboard != null)
                {
                    keyboard.OneTimeKeyboard = true;
                }

                SendAnswer(message.Chat, answer, keyboard);
            }
        }

        private static void SendAnswer(Chat chat, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(chat.Id, answer, replyMarkup: rm);
        }
    }
}