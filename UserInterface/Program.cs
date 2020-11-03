namespace UserInterface
{
    using System;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types.Enums;

    class Program
    {
        private static TelegramBotClient client;
        private static string token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";

        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                var newMesage = $"Hello, {message.Chat.FirstName}!";
                await client.SendTextMessageAsync(message.Chat.Id, newMesage);
            }
        }
    }
}