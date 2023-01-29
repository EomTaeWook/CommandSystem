using CLISystem.Attribude;
using CLISystem.Interface;
using System.Diagnostics;

namespace CLISystem.Cmd
{
    [Cmd("stop")]
    internal class StopProcssCmd : ICmdProcessor
    {
        public void Invoke(string[] args)
        {
            if(args.Length == 0)
            {
                return;
            }
            foreach(var item in args)
            {
                foreach(var process in Process.GetProcesses())
                {
                    if(process.MainWindowHandle == 0)
                    {
                        continue;
                    }
                    if(process.MainModule.FileName.Equals(item))
                    {
                        process.Kill();
                    }
                }
            }
        }
        public string Print()
        {
            return $"Process를 정지시킵니다.";
        }
    }
}
