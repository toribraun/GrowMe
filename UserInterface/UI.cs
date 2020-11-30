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

    public class UI
    {
        private App app;
        private ICommandExecutor executor;
        private KeyboardController keyboardController;
        private TelegramBotClient client;
        private string token;

        public UI(App app, ICommandExecutor executor, KeyboardController keyboardController)
        {
            this.app = app;
            this.executor = executor;
            this.keyboardController = keyboardController;
            this.token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";
        }

        public void Run()
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
            for (var i = 0; i < 5; i++)
            {
                executor.AddCommand(commands[i], commonStatus);
            }

            executor.AddCommand(commands[5], UserStatus.SendPlantName);
            executor.AddCommand(commands[5], UserStatus.SendPlantWateringInterval);
            executor.AddCommand(commands[5], UserStatus.DeletePlantByName);
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            app.SendNotification += (userId, plantName) => SendNotification(userId, plantName);
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                var answer = executor.ExecuteCommand(message);
                var keyboard = keyboardController.GetKeyboard(answer);
                SendAnswer(message.Chat, answer.AnswerText, keyboard);
            }
        }

        private void SendAnswer(Chat chat, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(chat.Id, answer, replyMarkup: rm);
        }

        private void SendNotification(long chatId, string plantName)
        {
            var answer = $"Самое время полить {plantName}!";
            client.SendTextMessageAsync(chatId, answer);
        }
    }
}