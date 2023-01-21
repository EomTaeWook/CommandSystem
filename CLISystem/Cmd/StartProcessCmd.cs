using CLISystem.Attribude;
using CLISystem.Interface;
using System.Diagnostics;

namespace CLISystem.Cmd
{
    [CmdAttribude("start")]
    internal class StartProcessCmd : ICmdProcessor
    {
        public void Invoke(string[] args)
        {
            if(args.Count() > 0)
            {
                Process.Start(args[0], args[1..]);
            }
        }

        public string Print()
        {
            return $"Process를 실행시킵니다.";
        }
    }
}
