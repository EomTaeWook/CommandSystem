using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using System.Text;

namespace CommandSystem.Cmd
{
    [MultipleCmd("help", "?", "h")]
    internal class HelpCmd : ICmdProcessor
    {
        private readonly HashSet<string> _cmdToMap;
        private readonly AliasTable _aliasTable;
        private readonly ServiceProvider _serviceProvider;
        public HelpCmd(AliasTable aliasTable, HashSet<string> cmdToMap, ServiceProvider serviceProvider)
        {
            _aliasTable = aliasTable;
            _cmdToMap = cmdToMap;
            _serviceProvider = serviceProvider;
        }

        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            foreach (var name in _cmdToMap)
            {
                var cmd = _serviceProvider.GetService<ICmdProcessor>(name);
                sb.AppendLine($"{name} : {cmd.Print()}");
            }
            foreach (var item in _aliasTable.GetDatas())
            {
                sb.AppendLine($"{item.Alias} : {item.Cmd}");
            }
            Console.WriteLine(sb.ToString());
            return Task.CompletedTask;
        }

        public string Print()
        {
            return "현재 등록된 명령어를 보여줍니다.";
        }
    }
}
