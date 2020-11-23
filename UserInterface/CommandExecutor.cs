using Domain;

namespace UserInterface
{
    using System;
    using System.Collections.Generic;
    using Application;
    using Infrastructure;
    using Telegram.Bot.Types;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly Dictionary<string, IUserCommand> allCommands;
        private readonly Dictionary<UserStatus, List<string>> commandsByStatus;
        private App app;

        public CommandExecutor(App app)
        {
            this.allCommands = new Dictionary<string, IUserCommand>();
            this.commandsByStatus = new Dictionary<UserStatus, List<string>>();
            this.allCommands.Add("/notacommand", new Commands.NonexistingCommand());
            this.AddCommand(new Commands.StartCommand());
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

        public string ExecuteCommand(Message message)
        {
            if (!this.app.UserExists(message.Chat.Id))
            {
                return this.Commands["/start"].Execute(message, this.app);
            }

            var commandNames = this.Commands.Keys;
            var userInput = message.Text;
            var status = this.app.GetUserStatus(message.Chat.Id);
            string answer = null;
            foreach (var commandName in commandNames)
            {
                if (userInput.ToLower().Contains(commandName))
                {
                    if (commandName.Contains("/"))
                    {
                        this.app.ChangeUserStatus(message.Chat.Id, UserStatus.DefaultStatus);
                        answer = this.Commands[commandName].Execute(message, this.app);
                        break;
                    }
                    else if (this.CommandsByStatus[status].Contains(commandName))
                    {
                        answer = this.Commands[commandName].Execute(message, this.app);
                        break;
                    }
                }
            }

            if (answer == null)
            {
                return this.Commands["/notacommand"].Execute(message, this.app);
            }

            return answer;
        }
    }
}
