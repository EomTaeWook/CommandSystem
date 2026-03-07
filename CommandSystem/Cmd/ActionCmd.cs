using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;

namespace CommandSystem.Cmd
{
    internal class ActionCmd(Func<string[], IActorRef, CancellationToken, Task> func, string desc) : ICommand
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
