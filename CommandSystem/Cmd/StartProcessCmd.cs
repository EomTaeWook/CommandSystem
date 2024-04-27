using CommandSystem.Attribute;
using CommandSystem.Interfaces;
using System.Diagnostics;

namespace CommandSystem.Cmd
{
    [Cmd("start")]
    internal class StartProcessCmd : ICommandAction
    {
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
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
