using CommandSystem.Attributes;
using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;
using System.Diagnostics;

namespace CommandSystem.Cmd
{
    [Command("start")]
    internal class StartProcessCmd : ICommand
    {
        public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
        {
            if (args.Length > 0)
            {
                return Task.Run(() =>
                {
                    Process.Start(args[0], args[1..]);
                }, cancellationToken);
            }
            return Task.CompletedTask;
        }

        public string Print()
        {
            return $"Process를 실행시킵니다.";
        }
    }
}
