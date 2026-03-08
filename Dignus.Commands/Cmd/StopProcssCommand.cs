using Dignus.Actor.Core.Actors;
using Dignus.Commands.Attributes;
using Dignus.Commands.Interfaces;
using System.Diagnostics;

namespace Dignus.Commands.Cmd
{
    [Command("stop")]
    internal class StopProcssCommand : ICommand
    {
        public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
        {
            if (args.Length == 0)
            {
                return Task.CompletedTask;
            }
            foreach (var item in args)
            {
                foreach (var process in Process.GetProcesses())
                {
                    if (process.MainWindowHandle == 0)
                    {
                        continue;
                    }
                    if (process.MainModule.FileName.Equals(item))
                    {
                        process.Kill();
                    }
                }
            }
            return Task.CompletedTask;
        }

        public string Print()
        {
            return $"Process를 정지시킵니다.";
        }
    }
}
