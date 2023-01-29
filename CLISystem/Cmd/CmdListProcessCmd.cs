using CLISystem.Attribude;
using CLISystem.Interface;
using CLISystem.Models;
using System.Text;

namespace CLISystem.Cmd
{
    [Cmd("?")]
    internal class CmdListProcessCmd : ICmdProcessor
    {
        private readonly Dictionary<string, ICmdProcessor> _cmdMap;
        private readonly AliasTable _aliasTable;
        public CmdListProcessCmd(Dictionary<string, ICmdProcessor> cmdList,
            AliasTable aliasTable)
        {
            _cmdMap = cmdList; 
            _aliasTable = aliasTable;
        }
        public void Invoke(string[] args)
        {
            var sb = new StringBuilder();
            foreach(var item in _cmdMap)
            {
                sb.AppendLine($"{item.Key} : {item.Value.Print()}");
            }
            sb.AppendLine("*** alias table ***");
            foreach (var item in _aliasTable.GetDatas())
            {
                sb.AppendLine($"{item.Alias} : {item.Cmd}");
            }

            Console.WriteLine(sb.ToString());
        }

        public string Print()
        {
            return $"현재 등록된 명령어를 출력합니다.";
        }
    }
}
