using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Commands.Messages;

namespace Dignus.Commands.Internals.Actors
{
    internal class LocalConsoleActor(IActorRef commandExecutionActorRef, string moduleName) : ActorBase
    {
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
                commandExecutionActorRef.Post(message);

                ShowPrompt();
            }
            return ValueTask.CompletedTask;
        }

        private void ShowPrompt()
        {
            Console.Write($"{moduleName} > ");
            var line = Console.ReadLine();
            var message = new RunCommandMessage(line);
            commandExecutionActorRef.Post(message, Self);
        }
    }
}
