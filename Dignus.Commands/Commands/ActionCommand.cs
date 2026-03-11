using Dignus.Actor.Core;
using Dignus.Commands.Interfaces;

namespace Dignus.Commands.Commands
{
    internal class ActionCommand(Func<string[], IActorRef, CancellationToken, Task> func, string desc) : ICommand
    {
        public string _desc = desc;

        public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
        {
            return func.Invoke(args, sender, cancellationToken);
        }

        public string Print()
        {
            return _desc;
        }
    }
}
