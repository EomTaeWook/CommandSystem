using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Models;
using System.Text;
using System.Text.Json;

namespace CommandSystem.Cmd
{
    [Cmd("alias")]
    internal class AliasCmd : ICmdProcessor
    {
        private readonly AliasTable _aliasTable;
        public AliasCmd(AliasTable aliasTable)
        {
            _aliasTable = aliasTable;
        }
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            if (args.Length == 0)
            {
                var sb = new StringBuilder();
                foreach (var item in _aliasTable.GetDatas())
                {
                    sb.AppendLine($"{item.Alias} : {item.Cmd}");
                }
                Console.WriteLine(sb.ToString());
                return Task.CompletedTask;
            }
            else
            {
                var aliasCmd = args[0];
                var cmd = string.Join(' ', args, 1, args.Length - 1);

                if(aliasCmd.Equals(cmd) == true)
                {
                    throw new Exception($"the alias and command are the same.");
                }

                if(string.IsNullOrEmpty(cmd))
                {
                    throw new Exception($"command is empty!");
                }

                _aliasTable.AddAlias(new AliasModel()
                {
                    Alias = aliasCmd,
                    Cmd = cmd
                });

                var datas = _aliasTable.GetDatas();
                var json = JsonSerializer.Serialize(datas);
                return File.WriteAllTextAsync(AliasTable.Path, json);
            }
        }

        public string Print()
        {
            return $"명령어의 별칭을 설정합니다. alias sp start process";
        }
    }
}
