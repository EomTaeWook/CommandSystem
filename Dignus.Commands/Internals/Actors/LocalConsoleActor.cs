using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Commands.Messages;

namespace Dignus.Commands.Internals.Actors
{
    internal class LocalConsoleActor(IActorRef commandExecutionActorRef, string moduleName) : ActorBase
    {
        private readonly List<string> _currentCommandPath = [];
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if(message is CommandResponseMessage commandResponse)
            {
                Console.WriteLine(commandResponse.Content);
            }
            else if(message is StartPromptMessage)
            {
                ShowPrompt();
            }
            else if(message is CancelCommandMessage)
            {
                commandExecutionActorRef.Post(message, Self);
            }
            else if (message is CompleteCommandMessage)
            {
                commandExecutionActorRef.Post(message, Self);

                ShowPrompt();
            }
            else if (message is ChangeDirectoryRequestMessage changeDirectoryRequestMessage)
            {
                HandleDirectoryChanged(changeDirectoryRequestMessage);
            }
            return ValueTask.CompletedTask;
        }
        private void HandleDirectoryChanged(ChangeDirectoryRequestMessage changeDirectoryRequest)
        {
            var result = CommandPathResolver.Resolve(_currentCommandPath, changeDirectoryRequest.Path);
            _currentCommandPath.Clear();
            _currentCommandPath.AddRange(result);
        }
        private void ShowPrompt()
        {
            var currentPath = "/";
            if (_currentCommandPath.Count > 0)
            {
                currentPath = string.Join("/", _currentCommandPath);
            }

            Console.Write($"{moduleName}:{currentPath}> ");
            Task.Run(() => 
            {
                var line = Console.ReadLine();
                var message = new RunCommandMessage(string.Join("/", _currentCommandPath), line);
                commandExecutionActorRef.Post(message, Self);
            });
        }
    }
}
