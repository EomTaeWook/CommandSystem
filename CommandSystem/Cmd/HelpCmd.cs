using CommandSystem.Attribute;
using CommandSystem.Interfaces;
using CommandSystem.Internals;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using System.Text;

namespace CommandSystem.Cmd
{
    [MultipleCmd("help", "?", "h")]
    internal class HelpCmd : ICommandAction
    {
        private readonly CommandTable _commandTable;
        private readonly AliasTable _aliasTable;
        private readonly ServiceContainer _serviceContainer;

        public HelpCmd(AliasTable aliasTable,
            CommandTable commandTable,
            ServiceContainer serviceContainer)
        {
            _aliasTable = aliasTable;
            _commandTable = commandTable;
            _serviceContainer = serviceContainer;
        }

        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            foreach (var name in _commandTable.GetCommandList())
            {
                var cmd = _serviceContainer.GetService<ICommandAction>(name);
                sb.AppendLine($"{name} : {cmd.Print()}");
            }
            sb.AppendLine();
            foreach (var name in _commandTable.GetLocalCommandList())
            {
                var cmd = _serviceContainer.GetService<ICommandAction>(name);
                sb.AppendLine($"{name} : {cmd.Print()}");
            }
            sb.AppendLine();
            foreach (var item in _aliasTable.GetDatas())
            {
                sb.AppendLine($"{item.Alias} : {item.Cmd}");
            }
            Console.Write(sb.ToString());
            return Task.CompletedTask;
        }

        public string Print()
        {
            return "현재 등록된 명령어를 보여줍니다.";
        }
    }
}
