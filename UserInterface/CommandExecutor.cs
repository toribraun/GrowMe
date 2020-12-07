using System.ComponentModel.Design;
using System.Linq;

namespace UserInterface
{
    using System;
    using System.Collections.Generic;
    using Application;
    using Domain;
    using Telegram.Bot.Types;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly Dictionary<string, IUserCommand> allCommands;
        private readonly Dictionary<UserStatus, List<string>> commandsByStatus;
        private readonly Dictionary<string, Action<long>> commandsByEvents;
        private App app;

        public event Action<long> OnGetPlants;
        public event Action<long> OnAddPlant;
        public event Action<long> OnGetPlantsToDelete;
        public event Action<long, string> OnStart;
        public event Action<long> OnCancel;
        public event Action<long, string> OnNonexistingCommand;
        public event Action<long, string> OnCheckUserExist;

        public CommandExecutor(App app)
        {
            this.allCommands = new Dictionary<string, IUserCommand>();
            this.commandsByStatus = new Dictionary<UserStatus, List<string>>();
            this.allCommands.Add("/notacommand", new Commands.NonexistingCommand());
            this.AddCommand(new Commands.StartCommand());
            OnStart += app.StartEvent;
            OnCancel += app.Cancel;
            OnGetPlantsToDelete += app.GetPlantsToDeleteEvent;
            OnGetPlants += app.GetPlantsByUserEvent;
            OnAddPlant += app.AddPlantByUserEvent;
            OnNonexistingCommand += (id, message) => app.HandleNonexistingCommand(id, message);
            OnCheckUserExist += (id, name) => app.CheckUserExistEvent(id, name);

            commandsByEvents = new Dictionary<string, Action<long>>()
            {
                {"мои растения", OnGetPlants},
                {"добавить", OnAddPlant},
                {"удалить", OnGetPlantsToDelete},
                {"отмена", OnCancel}
            };
            this.app = app;
        }

        public Dictionary<string, IUserCommand> Commands => this.allCommands;

        public Dictionary<UserStatus, List<string>> CommandsByStatus => this.commandsByStatus;

        public void AddCommand(IUserCommand command, params UserStatus[] possibleStatus)
        {
            foreach (var status in possibleStatus)
            {
                this.AddCommand(command, status);
            }
        }

        public void AddCommand(IUserCommand command, UserStatus status = UserStatus.DefaultStatus)
        {
            if (!this.CommandsByStatus.ContainsKey(status))
            {
                this.commandsByStatus.Add(status, new List<string>());
                this.commandsByStatus[status].Add("/notacommand");
            }

            foreach (var name in command.Names)
            {
                if (!this.CommandsByStatus[status].Contains(name))
                {
                    this.commandsByStatus[status].Add(name);
                }

                if (!this.Commands.ContainsKey(name))
                {
                    this.allCommands.Add(name, command);
                }
            }
        }

        public Answer ExecuteCommand(Message message)
        {
            if (!this.app.UserExists(message.Chat.Id))
            {
                return this.Commands["/start"].Execute(message, this.app);
            }
        
            var commandNames = this.Commands.Keys;
            var userInput = message.Text;
            var status = this.app.GetUserStatus(message.Chat.Id);
            Answer answer = null;
            foreach (var commandName in commandNames)
            {
                if (userInput.ToLower().Contains(commandName))
                {
                    // if (commandName.Contains("/"))
                    // {
                    //     this.app.Cancel(message.Chat.Id);
                    //     answer = this.Commands[commandName].Execute(message, this.app);
                    //     break;
                    // }
                    // else if (this.CommandsByStatus[status].Contains(commandName))
                    // {
                    //     answer = this.Commands[commandName].Execute(message, this.app);
                    //     break;
                    // }
                    //
                    commandsByEvents[commandName].Invoke(message.Chat.Id);
                }
            }
        
            if (answer == null)
            {
                return this.Commands["/notacommand"].Execute(message, this.app);
            }
        
            return answer;
        }

        public void ExecuteCommandMessage(Message message)
        {
            var userId = message.Chat.Id;
            var userName = message.Chat.Username;
            var textMessage = message.Text;
            OnCheckUserExist?.Invoke(userId, userName);

            var commandNames = this.commandsByEvents.Keys;
            var isEvent = false;

            foreach (var commandName in commandNames)
            {
                if (!textMessage.ToLower().Contains(commandName)) continue;
                commandsByEvents[commandName]?.Invoke(userId);
                isEvent = true;
                break;
            }

            if (!isEvent)
            {
                OnNonexistingCommand?.Invoke(userId, textMessage);
            }
        }
    }
}
