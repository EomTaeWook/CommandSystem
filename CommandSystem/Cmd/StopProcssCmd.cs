using CommandSystem.Attributes;
using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;
using System.Diagnostics;

namespace CommandSystem.Cmd
{
    [Command("stop")]
    internal class StopProcssCmd : ICommand
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
