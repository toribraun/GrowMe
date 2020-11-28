using Domain;

namespace UserInterface
{
    using Application;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.ReplyMarkups;

    public class KeyboardController
    {
        private App app;

        internal KeyboardController(App app)
        {
            this.app = app;
        }

        internal IReplyMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Мои растения"),
                new KeyboardButton("Добавить растение"),
                new KeyboardButton("Удалить растение")
            }, true);
        }

        internal IReplyMarkup GetCancelKeyboard()
        {
            return new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Отмена")
            }, true);
        }

        internal IReplyMarkup GetUserPlantsKeyboard(string[] plants)
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

        internal IReplyMarkup GetKeyboard(Message message)
        {
            var userStatus = this.app.GetUserStatus(message.Chat.Id);
            var user = this.app.GetUserById(message.Chat.Id);
            if (userStatus == UserStatus.DeletePlantByName)
            {
                return this.GetUserPlantsKeyboard(this.app
                        .GetPlantsByUser(user)
                        .Split("\n"));
            }
            else if (userStatus == UserStatus.DefaultStatus)
            {
                return this.GetMainMenuKeyboard();
            }
            else
            {
                return this.GetCancelKeyboard();
            }
        }
    }
}
