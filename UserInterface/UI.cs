namespace UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Application;
    using Application.Replies;
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

        public UI(TelegramBotClient client, ICommandExecutor executor, KeyboardController keyboardController)
        {
            this.client = client;
            this.executor = executor;
            this.keyboardController = keyboardController;
        }

        public void Run()
        {
            client.OnMessage += BotOnMessageReceived;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            switch (message?.Type)
            {
                case MessageType.Text:
                    executor.ExecuteCommandMessage(message);
                    break;
                case MessageType.Photo:
                    executor.ExecutePhoto(message);
                    break;
            }
        }

        public void BuildMessageToUser(IReply reply)
        {
            if (reply.GetType() == typeof(ReplyOnGetPlantPhoto))
            {
                if (((ReplyOnGetPlantPhoto)reply).IsExist)
                {
                    SendAnswerWithPhoto((ReplyOnGetPlantPhoto)reply);
                    return;
                }
            }

            SendAnswer(reply.UserId, GetTextAnswer(reply), GetKeyboard(reply));
        }

        private string GetTextAnswer(IReply reply)
        {
            var answerText = "Я не понимаю тебя. Если нужна справка, введи /help!";
            if (reply.GetType() == typeof(ReplyOnGetPlants))
            {
                if (!((ReplyOnGetPlants)reply).PlantsName.Any())
                {
                    answerText = "У тебя пока не записано растений!";
                }
                else
                {
                    answerText = "Вот все твои растения, про которые мне известно.\n\n" +
                                 $"{string.Join('\n', ((ReplyOnGetPlants)reply).PlantsName)}\n\n" +
                                 $"Выбери растение, о котором хочешь узнать подробнее.\n" +
                                 $"Нажми «Отмена», чтобы вернуться в главное меню.";
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
                answerText = plantsNames.Length > 0 ? "Какое растение ты хочешь удалить?" :
                    "У тебя пока не записано растений!";
            }
            else if (reply.GetType() == typeof(ReplyOnDeletedPlant))
            {
                answerText = ((ReplyOnDeletedPlant)reply).IsDeleted ?
                    $"Растение {((ReplyOnDeletedPlant)reply).DeletedPlantName} удалено!" :
                    "У тебя нет растения с таким именем!";
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
            }
            else if (reply.GetType() == typeof(ReplyOnSetWateringInterval))
            {
                answerText = "Поздравляю, твоё растение добавлено!";
            }
            else if (reply.GetType() == typeof(ReplyOnGetPlantPhoto) && !((ReplyOnGetPlantPhoto)reply).IsExist)
            {
                answerText = "У этого растения пока нет фотографий.\n\n" +
                             "Чтобы добавить фотографию своего растения, просто отправь её мне. " +
                             "Не забудь добавить в описании имя растения, иначе я тебя не пойму :(";
            }
            else if (reply.GetType() == typeof(ReplyOnSetPlantPhoto))
            {
                answerText = ((ReplyOnSetPlantPhoto)reply).IsAdded ?
                    $"Фотография для растения «{((ReplyOnSetPlantPhoto)reply).PlantName}» успешно добавлена" :
                    "Что-то пошло не так :(";
            }
            else if (reply.GetType() == typeof(ReplyOnSchedule))
            {
                answerText = ((ReplyOnSchedule)reply).ScheduleMessage;
            }
            else if (reply.GetType() == typeof(ReplyOnHelp))
            {
                answerText = "Мои растения – информация о всех твоих растениях, включая первую и последнюю фотографию.\n" +
                             "Чтобы добавить новую фотографию растения, просто отправь её боту, подписав имя растения.\n\n" +
                             "Для добавления и удаления растений используй соответсвующие кнопки.\n\n" +
                             "Расписание – расписание полива растений на неделю.";
            }

            return answerText;
        }

        private IReplyMarkup GetKeyboard(IReply reply)
        {
            if (reply.GetType() == typeof(ReplyOnGetPlants))
                return keyboardController.GetUserPlantsKeyboard(((ReplyOnGetPlants)reply).PlantsName.ToArray());
            if (reply.GetType() == typeof(ReplyOnGetPlantsToDelete))
                return keyboardController.GetUserPlantsKeyboard(((ReplyOnGetPlantsToDelete)reply).PlantsName.ToArray());
            if (reply.GetType() == typeof(ReplyOnWantedAddPlant) || reply.GetType() == typeof(ReplyOnSetPlantName))
                return keyboardController.GetCancelKeyboard();
            return keyboardController.GetMainMenuKeyboard();
        }

        private void SendAnswer(long userId, string answer, IReplyMarkup rm)
        {
            client.SendTextMessageAsync(userId, answer, replyMarkup: rm);
        }

        private async void SendAnswerWithPhoto(ReplyOnGetPlantPhoto reply)
        {
            var text = $"{reply.PlantName}\n\nДата добавления: {reply.AddingDatetime.Date.ToShortDateString()}";
            var photos = new List<IAlbumInputMedia>();

            if (!string.IsNullOrEmpty(reply.FirstPhotoId))
                photos.Add(new InputMediaPhoto(new InputMedia(reply.FirstPhotoId)));

            if (!string.IsNullOrEmpty(reply.LastPhotoId))
                photos.Add(new InputMediaPhoto(new InputMedia(reply.LastPhotoId)));

            if (photos.Any())
            {
                await client.SendMediaGroupAsync(photos, reply.UserId);
                SendAnswer(reply.UserId, text, keyboardController.GetMainMenuKeyboard());
            }
        }

        public void SendNotification(long chatId, string plantName)
        {
            var answer = $"Самое время полить {plantName}!\n\n" +
                         "Не забудь добавить новую фотографию своего растения. " +
                         "Так ты сможешь наглядно увидеть его рост.\n\n" +
                         $"Чтобы добавить новую фотографию, просто отправь её боту с текстом: {plantName}.";
            client.SendTextMessageAsync(chatId, answer);
        }
    }
}