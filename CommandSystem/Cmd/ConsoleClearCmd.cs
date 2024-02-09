using CommandSystem.Attribute;
using CommandSystem.Interface;

namespace CommandSystem.Cmd
{
    [LocalCmd("clr")]
    internal class ConsoleClearCmd : ICommandAction
    {
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
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
