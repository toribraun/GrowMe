using System.Linq;
using Application.Replies;

namespace UserInterface
{
    using System;
    using Application;
    using Telegram.Bot;
    using Telegram.Bot.Args;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;

    public class UI
    {
        private ICommandExecutor executor;
        private KeyboardController keyboardController;
        private TelegramBotClient client;
        private string token;

        public UI(ICommandExecutor executor, KeyboardController keyboardController)
        {
            this.executor = executor;
            this.keyboardController = keyboardController;
            this.token = "1017290663:AAF1ZG3q_hGOZF5rCfJDh-WbT-NLgGGMW98";
        }

        public void Run()
        {
            // var commands = new IUserCommand[]
            //     {
            //         new Commands.StartCommand(),
            //         new Commands.HelpCommand(),
            //         new Commands.GetPlantsCommand(),
            //         new Commands.AddPlantCommand(),
            //         new Commands.DeletePlantCommand(),
            //         new Commands.CancelCommand()
            //     };
            // var commonStatus = new UserStatus[] { UserStatus.DefaultStatus, UserStatus.SendUserName };
            // for (var i = 0; i < 5; i++)
            // {
            //     executor.AddCommand(commands[i], commonStatus);
            // }
            //
            // executor.AddCommand(commands[5], UserStatus.SendPlantName);
            // executor.AddCommand(commands[5], UserStatus.SendPlantWateringInterval);
            // executor.AddCommand(commands[5], UserStatus.DeletePlantByName);
            client = new TelegramBotClient(token);
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message?.Type == MessageType.Text)
            {
                executor.ExecuteCommandMessage(message);
                // var answer = executor.ExecuteCommand(message);
                // var keyboard = keyboardController.GetKeyboard(answer);
                // SendAnswer(message.Chat, answer.AnswerText, keyboard);
            }
        }

        public void BuildMessageToUser(IReply reply)
        {
            var keyboard = keyboardController.GetMainMenuKeyboard();
            var answerText = "Я не понимаю тебя. Если нужна справка, введи /help!";
            if (reply.GetType() == typeof(ReplyOnGetPlants))
            {
                if (!((ReplyOnGetPlants)reply).PlantsName.Any())
                {
                    answerText = "У тебя пока не записано растений!";
                }
                else
                {
                    answerText = "Вот все твои растения, про которые мне известно:\n\n" +
                                 $"{string.Join('\n', ((ReplyOnGetPlants)reply).PlantsName)}";
                }
            }
            else if (reply.GetType() == typeof(ReplyOnStart))
            {
                if (((ReplyOnStart)reply).IsAdded)
                {
                    answerText = $"Привет, {((ReplyOnStart)reply).UserName}!\n" +
                                 "Я - бот, который будет помогать тебе в уходе за растениями.\n" +
                                 "Введи /help для справки.";
                }
                else
                {
                    answerText = $"Снова здравствуй, {((ReplyOnStart)reply).UserName}! Если нужна справка - введи /help";
                }
            }
            else if (reply.GetType() == typeof(ReplyOnCancel))
            {
                answerText = "Ну хорошо, ты снова в главном меню";
            }
            else if (reply.GetType() == typeof(ReplyOnGetPlantsToDelete))
            {
                var plantsNames = ((ReplyOnGetPlantsToDelete)reply).PlantsName.ToArray();
                if (plantsNames.Length > 0)
                {
                    answerText = "Какое растение ты хочешь удалить?";
                    keyboard = keyboardController.GetUserPlantsKeyboard(plantsNames.ToArray());
                }
                else
                {
                    answerText = "У тебя пока не записано растений!";
                }
            }
            else if (reply.GetType() == typeof(ReplyOnDeletedPlant))
            {
                if (((ReplyOnDeletedPlant)reply).IsDeleted)
                {
                    answerText = $"Растение {((ReplyOnDeletedPlant)reply).DeletedPlantName} удалено!";
                }
                else
                {
                    answerText = "У тебя нет растения с таким именем!";
                }
            }
            else if (reply.GetType() == typeof(ReplyOnWantedAddPlant))
            {
                if (((ReplyOnWantedAddPlant)reply).TriedInvalidName)
                {
                    answerText = "Я не могу записать такое имя. " +
                                 "Пожалуйста, убедись, что растения с таким именем еще нет, " +
                                 "оно написано в одну строку и короче 25 символов.";
                }
                else
                {
                    answerText = "Как назовём твоё растение?";
                }

                keyboard = keyboardController.GetCancelKeyboard();
            }
            else if (reply.GetType() == typeof(ReplyOnSetPlantName))
            {
                if (((ReplyOnSetPlantName)reply).TriedInvalidInterval)
                {
                    answerText = "Пожалуйста, введи целое положительное число.";
                }
                else
                {
                    answerText = "Как часто нужно поливать твоё растение? Укажи интервал в сутках.\n" +
                                 "Например, если твоё растение нужно поливать каждые три дня, напиши: 3.";
                }

                keyboard = keyboardController.GetCancelKeyboard();
            }
            else if (reply.GetType() == typeof(ReplyOnSetWateringInterval))
            {
                answerText = "Поздравляю, твоё растение добавлено!";
            }
            else if (reply.GetType() == typeof(ReplyOnHelp))
            {
                answerText = "Здесь должна быть справка";
            }

            SendAnswer(reply.UserId, answerText, keyboard);
        }

        private void SendAnswer(Chat chat, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(chat.Id, answer, replyMarkup: rm);
        }

        private void SendAnswer(long userId, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(userId, answer, replyMarkup: rm);
        }

        public void SendNotification(long chatId, string plantName)
        {
            var answer = $"Самое время полить {plantName}!";
            client.SendTextMessageAsync(chatId, answer);
        }
    }
}