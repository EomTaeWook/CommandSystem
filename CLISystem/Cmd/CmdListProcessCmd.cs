using CLISystem.Attribude;
using CLISystem.Interface;
using System.Text;

namespace CLISystem.Cmd
{
    [CmdAttribude("?")]
    internal class CmdListProcessCmd : ICmdProcessor
    {
        readonly Dictionary<string, ICmdProcessor> _cmdMap;
        public CmdListProcessCmd(Dictionary<string, ICmdProcessor> cmdList)
        {
            _cmdMap = cmdList;
        }
        public void Invoke(string[] args)
        {
            var sb = new StringBuilder();
            foreach(var item in _cmdMap)
            {
                sb.AppendLine($"{item.Key} : {item.Value.Print()}");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(sb.ToString());
        }

        public string Print()
        {
            return $"현재 등록된 명령어를 출력합니다.";
        }
    }
}
