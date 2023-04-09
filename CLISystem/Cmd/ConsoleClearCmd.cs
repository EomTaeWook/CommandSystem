using CLISystem.Attribude;
using CLISystem.Interface;

namespace CLISystem.Cmd
{
    [Cmd("clr")]
    internal class ConsoleClearCmd : ICmdProcessor
    {
        public Task InvokeAsync(string[] args)
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
