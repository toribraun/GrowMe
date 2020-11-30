using Domain;

namespace UserInterface
{
    using Application;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.ReplyMarkups;

    public class KeyboardController
    {
        private App app;

        public KeyboardController(App app)
        {
            this.app = app;
        }

        private IReplyMarkup GetMainMenuKeyboard()
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

        internal IReplyMarkup GetKeyboard(Answer answer)
        {
            var user = this.app.GetUserById(answer.UserId);
            if (answer.Status == UserStatus.DeletePlantByName)
            {
                return this.GetUserPlantsKeyboard(this.app
                        .GetPlantsByUser(user)
                        .Split("\n"));
            }

            if (answer.Status == UserStatus.DefaultStatus)
            {
                return this.GetMainMenuKeyboard();
            }

            return this.GetCancelKeyboard();
        }
    }
}
