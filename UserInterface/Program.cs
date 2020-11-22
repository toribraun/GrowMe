using Telegram.Bot.Types.ReplyMarkups;

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
        private static IReplyMarkup mainMenuKeyboard = new ReplyKeyboardMarkup(
            new[]
        {
            new KeyboardButton("Мои растения"),
            new KeyboardButton("Добавить растение"),
            new KeyboardButton("Удалить растение")
        }, true);

        private static IReplyMarkup cancelKeyboard = new ReplyKeyboardMarkup(
            new[]
        {
            new KeyboardButton("Отмена")
        }, true);

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
            var answer = "Я пока не знаю, что с этим делать.";
            ReplyKeyboardMarkup keyboard = null;
            if (message?.Type == MessageType.Text)
            {
                var messageText = message.Text;
                if (messageText.Equals("/start") || !app.UserExists(message.Chat.Id))
                {
                    answer = GreetAtStart(message.Chat);
                }
                else if (messageText == "/help")
                {
                    answer = Help(message.Chat);
                }
                else if (messageText == "Отмена")
                {
                    app.Cancel(message.Chat.Id);
                    answer = "Главное меню";
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
                else if (messageText.ToLower().Contains("удалить"))
                {
                    var plants = app
                        .GetPlantsByUser(new Infrastructure.User(message.Chat.Id, message.Chat.FirstName))
                        .Split("\n");
                    if (plants[0] == string.Empty)
                    {
                        answer = "У тебя пока не записано растений!";
                    }
                    else
                    {
                        app.ChangeUserStatus(message.Chat.Id, UserStatus.DeletePlantByName);
                        answer = "Какое растение нужно удалить?";
                        keyboard = GetUserPlantsKeyboard(plants);
                    }
                }
                else if (app.GetUserStatus(message.Chat.Id) == UserStatus.DeletePlantByName)
                {
                    if (app.DeletePlant(message.Chat.Id, messageText))
                    {
                        answer = $"Растение {messageText} удалено!";
                    }
                    else
                    {
                        answer = "У тебя нет растения с таким именем!";
                        keyboard = GetUserPlantsKeyboard(app
                            .GetPlantsByUser(new Infrastructure.User(message.Chat.Id, message.Chat.FirstName))
                            .Split("\n"));
                    }
                }
                else if (messageText.ToLower().Contains("добавить"))
                {
                    app.ChangeUserStatus(message.Chat.Id, UserStatus.SendPlantName);
                    answer = "Как назовём твоё растение?";
                }
                else if (app.GetUserStatus(message.Chat.Id) == UserStatus.SendPlantName)
                {
                    if (app.SetNewPlantName(message.Chat.Id, messageText))
                    {
                        answer = "Как часто нужно поливать твоё растение? Укажи интервал в сутках.\n" +
                                 "Например, если твоё растение нужно поливать каждые три дня, отправь 3.";
                    }
                    else
                    {
                        answer = "У тебя уже есть растение с таким именем! Придумай другое.";
                    }
                }
                else if (app.GetUserStatus(message.Chat.Id) == UserStatus.SendPlantWateringInterval)
                {
                    answer = "Пожалуйста, введи целое положительное число";
                    try
                    {
                        if (int.TryParse(messageText, out var interval))
                        {
                            app.AddNewPlantFromActivePlantWithWateringInterval(message.Chat.Id, interval);
                            answer = "Поздравляем, твоё растение добавлено!";
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                if (keyboard == null)
                {
                    if (app.GetUserStatus(message.Chat.Id) == UserStatus.DefaultStatus)
                    {
                        keyboard = (ReplyKeyboardMarkup)mainMenuKeyboard;
                    }
                    else
                    {
                        keyboard = (ReplyKeyboardMarkup)cancelKeyboard;
                    }
                }

                keyboard.OneTimeKeyboard = true;

                SendAnswer(message.Chat, answer, keyboard);
            }
        }

        private static ReplyKeyboardMarkup GetUserPlantsKeyboard(string[] plants)
        {
            ReplyKeyboardMarkup keyboard;
            var buttons = new KeyboardButton[plants.Length + 1];
            for (var i = 0; i < buttons.Length - 1; i++)
            {
                buttons[i] = new KeyboardButton(plants[i]);
            }

            buttons[^1] = "Отмена";
            keyboard = new ReplyKeyboardMarkup(buttons, true);
            return keyboard;
        }

        private static string GreetAtStart(Chat chat)
        {
            var user = new Infrastructure.User(chat.Id, chat.FirstName);
            if (app.AddUser(user))
            {
                return $"Привет, {chat.FirstName}!\n" +
                    $"Я - бот, который будет помогать тебе в уходе за растениями.\n" +
                    $"Введи /help для справки.";
            }

            return $"Снова здравствуй, {chat.FirstName}! Если нужна справка - введи /help";
        }

        private static string Help(Chat chat)
        {
            return "Здесь должна быть справка, но её пока нет";
        }

        private static void SendAnswer(Chat chat, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(chat.Id, answer, replyMarkup: rm);
        }
    }
}