using CLISystem.Attribude;
using CLISystem.Interface;

namespace CLISystem.Cmd
{
    [CmdAttribude("clr")]
    internal class ConsoleClearCmd : ICmdProcessor
    {
        public void Invoke(string[] args)
        {
            Console.Clear();
        }

        public string Print()
        {
            return "console 창을 지웁니다.";
        }
    }
}
