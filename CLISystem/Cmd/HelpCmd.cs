using CLISystem.Attribude;
using CLISystem.Interface;
using CLISystem.Models;
using System.Text;

namespace CLISystem.Cmd
{
    [MultipleCmd("help","?","h")]
    internal class HelpCmd : ICmdProcessor
    {
        private readonly ProcessorNames _processorNames; 
        private readonly AliasTable _aliasTable;
        public HelpCmd(ProcessorNames processorNames,
            AliasTable aliasTable)
        {
            _processorNames = processorNames; 
            _aliasTable = aliasTable;
        }
        public void Invoke(string[] args)
        {
            var sb = new StringBuilder();
            foreach (var name in _processorNames)
            {
                var processor = _processorNames.GetCmdProcessor(name);
                sb.AppendLine($"{name} : {processor.Print()}");
            }
            foreach (var item in _aliasTable.GetDatas())
            {
                sb.AppendLine($"{item.Alias} : {item.Cmd}");
            }
            Console.WriteLine(sb.ToString());
        }

        public string Print()
        {
            return "현재 등록된 명령어를 보여줍니다.";
        }
    }
}
