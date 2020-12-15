namespace UserInterface
{
    using System.Collections.Generic;
    using System.Linq;
    using Application;
    using Telegram.Bot.Types.ReplyMarkups;

    public class KeyboardController
    {
        private App app;

        public KeyboardController(App app)
        {
            this.app = app;
        }

        public IReplyMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(
            new List<KeyboardButton[]>()
            {
                new KeyboardButton[] { new KeyboardButton("Мои растения") },
                new KeyboardButton[] { new KeyboardButton("Добавить растение") },
                new KeyboardButton[] { new KeyboardButton("Удалить растение") },
                new KeyboardButton[] { new KeyboardButton("Расписание") }
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
            var buttons = plants
                .Select(p => new KeyboardButton[] { p })
                .Append(new KeyboardButton[] { "Отмена" });
            keyboard = new ReplyKeyboardMarkup(buttons, true);
            return keyboard;
        }
    }
}
