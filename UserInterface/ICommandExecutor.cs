using Domain;

namespace UserInterface
{
    using System.Collections.Generic;
    using Telegram.Bot.Types;

    public interface ICommandExecutor
    {
        public Dictionary<string, IUserCommand> Commands { get; }

        public void AddCommand(IUserCommand command, UserStatus status);

        public void AddCommand(IUserCommand command, params UserStatus[] status);
        public void ExecutePhoto(Message message);

        // public Answer ExecuteCommand(Message message);

        public void ExecuteCommandMessage(Message message);
    }
}
