using CommandSystem.Messages;
using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;

namespace CommandSystem.Actors
{
    internal class LocalConsoleActor(IActorRef commandExecutionActorRef, string moduleName) : ActorBase
    {
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if(message is CommandResponseMessage commandResponse)
            {
                Console.WriteLine(commandResponse);
            }
            else if(message is StartPromptMessage)
            {
                DisplayPrompt();
            }
            else if(message is CancelCommandMessage)
            {
                commandExecutionActorRef.Post(message, Self);
            }
            return ValueTask.CompletedTask;
        }

        private void DisplayPrompt()
        {
            Console.Write($"{moduleName} > ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                _ = Task.Run(DisplayPrompt);
                return;
            }
            else if (string.IsNullOrEmpty(line.Trim()))
            {
                _ = Task.Run(DisplayPrompt);
                return;
            }

            var message = new RunCommandMessage(line);
            commandExecutionActorRef.Post(message, Self);
        }
    }
}
