namespace UserInterface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Application;
    using Domain;
    using Telegram.Bot.Types;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly Dictionary<string, EventHandler<long>> commandsByEvents;
        private App app;

        public event EventHandler<long> OnGetPlants;
        public event EventHandler<long> OnAddPlant;
        public event EventHandler<long> OnGetPlantsToDelete;
        public event EventHandler<EventUserArgs> OnStart;
        public event EventHandler<long> OnCancel;
        public event EventHandler<long> OnHelp;
        public event EventHandler<EventCommandArgs> OnNonexistingCommand;
        public event EventHandler<EventUserArgs> OnCheckUserExist;
        public event EventHandler<EventPhotoArgs> OnSendPhoto;
        public event EventHandler<long> OnSchedule;

        public class EventCommandArgs
        {
            public long UserId { get; }
            public string Message { get; }

            public EventCommandArgs(long userId, string message)
            {
                UserId = userId;
                Message = message;
            }
        }

        public class EventUserArgs
        {
            public long UserId { get; }
            public string UserName { get; }

            public EventUserArgs(long userId, string userName)
            {
                UserId = userId;
                UserName = userName;
            }
        }

        public class EventPhotoArgs
        {
            public long UserId { get; }
            public string PlantName { get; }
            public string PhotoId { get; }

            public EventPhotoArgs(long userId, string plantName, string photoId)
            {
                UserId = userId;
                PlantName = plantName;
                PhotoId = photoId;
            }
        }

        public CommandExecutor(App app)
        {
            OnStart += (sender, eventArgsStart) => app.StartEvent(eventArgsStart.UserId, eventArgsStart.UserName);
            OnCancel += (sender, userId) => app.Cancel(userId);
            OnGetPlantsToDelete += (sender, userId) => app.GetPlantsToDeleteEvent(userId);
            OnGetPlants += (sender, userId) => app.GetPlantsByUserEvent(userId);
            OnAddPlant += (sender, userId) => app.AddPlantByUserEvent(userId);
            OnNonexistingCommand += (sender, commandArgs) =>
                app.HandleNonexistingCommand(commandArgs.UserId, commandArgs.Message);
            OnCheckUserExist += (sender, checkUserArgs) =>
                app.CheckUserExistEvent(checkUserArgs.UserId, checkUserArgs.UserName);
            OnHelp += (sender, userId) => app.GetHelp(userId);
            OnSendPhoto += (sender, photoArgs) =>
                app.AddPlantPhotoEvent(photoArgs.UserId, photoArgs.PlantName, photoArgs.PhotoId);
            OnSchedule += (sender, userId) => app.GetWeekSchedule(userId);

            commandsByEvents = new Dictionary<string, EventHandler<long>>()
            {
                { "мои растения", OnGetPlants },
                { "добавить", OnAddPlant },
                { "удалить", OnGetPlantsToDelete },
                { "отмена", OnCancel },
                { "/help", OnHelp },
                { "расписание", OnSchedule }
            };
        }

        public void ExecuteCommandMessage(Message message)
        {
            var userId = message.Chat.Id;
            var userName = message.Chat.FirstName;
            var textMessage = message.Text;
            OnCheckUserExist?.Invoke(this, new EventUserArgs(userId, userName));

            var commandNames = this.commandsByEvents.Keys;
            var isEvent = false;

            foreach (var commandName in commandNames
                .Where(commandName => textMessage.ToLower().Contains(commandName)))
            {
                commandsByEvents[commandName]?.Invoke(this, userId);
                isEvent = true;
                break;
            }

            if (!isEvent)
            {
                OnNonexistingCommand?.Invoke(this, new EventCommandArgs(userId, textMessage));
            }
        }

        public void ExecutePhoto(Message message)
        {
            OnSendPhoto?.Invoke(
                this,
                new EventPhotoArgs(message.Chat.Id, message.Caption, message.Photo.Last().FileId));
        }

        // public Answer ExecuteCommand(Message message)
        // {
            // if (!this.app.UserExists(message.Chat.Id))
            // {
            //     return this.Commands["/start"].Execute(message, this.app);
            // }
            //
            // var commandNames = this.Commands.Keys;
            // var userInput = message.Text;
            // var status = this.app.GetUserStatus(message.Chat.Id);
            // Answer answer = null;
            // foreach (var commandName in commandNames)
            // {
            //     if (userInput.ToLower().Contains(commandName))
            //     {
            //         // if (commandName.Contains("/"))
            //         // {
            //         //     this.app.Cancel(message.Chat.Id);
            //         //     answer = this.Commands[commandName].Execute(message, this.app);
            //         //     break;
            //         // }
            //         // else if (this.CommandsByStatus[status].Contains(commandName))
            //         // {
            //         //     answer = this.Commands[commandName].Execute(message, this.app);
            //         //     break;
            //         // }
            //         //
            //         commandsByEvents[commandName].Invoke(message.Chat.Id);
            //     }
            // }
            //
            // if (answer == null)
            // {
            //     return this.Commands["/notacommand"].Execute(message, this.app);
            // }
            // return null;
        // }
    }
}
