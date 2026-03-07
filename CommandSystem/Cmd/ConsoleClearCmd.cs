using CommandSystem.Attributes;
using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;

namespace CommandSystem.Cmd
{
    [LocalCommand("clr")]
    internal class ConsoleClearCmd : ICommand
    {
        public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
        {
            Console.Clear();
            return Task.CompletedTask;
        }

        public string Print()
        {
            return "console 창을 지웁니다.";
        }
    }
}
