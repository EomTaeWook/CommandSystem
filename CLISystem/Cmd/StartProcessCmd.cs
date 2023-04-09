using CLISystem.Attribude;
using CLISystem.Interface;
using System.Diagnostics;

namespace CLISystem.Cmd
{
    [Cmd("start")]
    internal class StartProcessCmd : ICmdProcessor
    {
        public async Task InvokeAsync(string[] args)
        {
            if (args.Length > 0)
            {
                await Task.Run(() => {
                    Process.Start(args[0], args[1..]);
                });
            }
        }

        public string Print()
        {
            return $"Process를 실행시킵니다.";
        }
    }
}
