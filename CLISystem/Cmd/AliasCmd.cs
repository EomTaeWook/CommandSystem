using CLISystem.Attribude;
using CLISystem.Interface;
using CLISystem.Models;
using System.Text;
using System.Text.Json;

namespace CLISystem.Cmd
{
    [CmdAttribude("alias")]
    internal class AliasCmd : ICmdProcessor
    {
        private readonly AliasTable _aliasTable;
        public AliasCmd(AliasTable aliasTable)
        {
            _aliasTable = aliasTable;
        }

        public async void Invoke(string[] args)
        {
            if (args.Length == 0)
            {
                var sb = new StringBuilder();
                foreach (var item in _aliasTable.GetDatas())
                {
                    sb.AppendLine($"{item.Alias} : {item.Cmd}");
                }
                Console.WriteLine(sb.ToString());
            }
            else
            {
                var aliasCmd = args[0];
                var cmd = String.Join(' ', args, 1, args.Length -1);

                _aliasTable.AddAlias(new AliasModel()
                {
                    Alias = aliasCmd,
                    Cmd = cmd
                });

                var datas = _aliasTable.GetDatas();
                var json = JsonSerializer.Serialize(datas);
                await File.WriteAllTextAsync(AliasTable.Path, json);
            }
        }

        public string Print()
        {
            return $"명령어의 별칭을 설정합니다. alias sp=start process";
        }
    }
}
