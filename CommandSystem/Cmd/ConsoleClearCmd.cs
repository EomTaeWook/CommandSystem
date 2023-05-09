using CommandSystem.Attribude;
using CommandSystem.Interface;

namespace CommandSystem.Cmd
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
