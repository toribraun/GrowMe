namespace UserInterface
{
    using System;
    using Application;
    using Infrastructure;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;


    class Program
    {
        private static TelegramBotClient client;
        private static string token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";
        public static App app = new App();

        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            string answer = "Я пока не знаю, что с этим делать.";
            if (message?.Type == MessageType.Text)
            {
                var messageText = message.Text;
                if (messageText == "/start")
                {
                    answer = GreetAtStart(message.Chat);
                }
                else if (messageText == "/help")
                {
                    answer = Help(message.Chat);
                }
                else if (messageText.ToLower().Contains("мои растения"))
                {
                    var plants = app.GetPlantsByUser(new Infrastructure.User(message.Chat.Id, message.Chat.FirstName));
                    if (plants.Length == 0)
                    {
                        answer = "У тебя пока не записано растений!";
                    }
                    else 
                    {
                        answer = plants;
                    }
                }
                else if (messageText.ToLower().Contains("добавить"))
                {
                    answer = "Это ещё не готово:(";
                }
                SendAnswer(message.Chat, answer);
            }
        }

        private static string GreetAtStart(Chat chat)
        {
            var user = new Infrastructure.User(chat.Id, chat.FirstName);
            if (app.AddUser(user))
            {
                return $"Привет, {chat.FirstName}! " +
                    $"Я - бот, который будет помогать тебе в уходе за растениями. " +
                    $"Введи /help для справки.";
            }
            return $"Снова здравствуй, {chat.FirstName}! Если нужна справка - введи /help";
        }

        private static string Help(Chat chat)
        {
            return "Здесь должна быть справка, но её пока нет";
        }

        private static void SendAnswer(Chat chat, string answer)
        {
            client.SendTextMessageAsync(chat.Id, answer);
        }
    }
}