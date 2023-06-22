using CommandSystem.Attribude;
using CommandSystem.Interface;
using System.Diagnostics;

namespace CommandSystem.Cmd
{
    [Cmd("stop")]
    internal class StopProcssCmd : ICmdProcessor
    {
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
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
