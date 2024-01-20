﻿using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Internal;
using CommandSystem.Models;
using System.Text;

namespace CommandSystem.Cmd
{
    [MultipleCmd("help", "?", "h")]
    internal class HelpCmd : ICmdProcessor
    {
        private readonly CommandTable _commandTable;
        private readonly AliasTable _aliasTable;
        private readonly CommandServiceContainer _commandContainer;
        public HelpCmd(AliasTable aliasTable, CommandTable commandTable, CommandServiceContainer commandContainer)
        {
            _aliasTable = aliasTable;
            _commandTable = commandTable;
            _commandContainer = commandContainer;
        }

        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            foreach (var name in _commandTable.GetCommandList())
            {
                var cmd = _commandContainer.Resolve<ICmdProcessor>(name);
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
