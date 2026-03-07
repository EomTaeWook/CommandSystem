using CommandSystem.Attributes;
using CommandSystem.Interfaces;
using CommandSystem.Internals;
using CommandSystem.Messages;
using Dignus.Actor.Core.Actors;
using System.Text;

namespace CommandSystem.Cmd
{
    [MultipleCommand("help", "?", "h")]
    internal class HelpCmd(AliasTable aliasTable,
        CommandTable commandTable,
        IServiceProvider serviceProvider) : ICommand
    {
        public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
        {
            var sb = new StringBuilder();

            foreach (var name in commandTable.GetCommandList())
            {
                var commandType = commandTable.GetCommandType(name);
                var command = (ICommand)serviceProvider.GetService(commandType);
                sb.AppendLine($"{name} : {command.Print()}");
            }

            if(commandTable.GetLocalCommandList().Any())
            {
                sb.AppendLine();
            }
            
            foreach (var name in commandTable.GetLocalCommandList())
            {
                var commandType = commandTable.GetCommandType(name);
                var command = (ICommand)serviceProvider.GetService(commandType);
                sb.AppendLine($"{name} : {command.Print()}");
            }

            if(aliasTable.Alias.Count > 0)
            {
                sb.AppendLine();
            }

            foreach (var item in aliasTable.GetDatas())
            {
                sb.AppendLine($"{item.Alias} : {item.Cmd}");
            }

            if (sb.Length > 0)
            {
                sb.Length -= Environment.NewLine.Length;
            }

            sender.Post(new CommandResponseMessage()
            {
                Content = sb.ToString()
            });

            return Task.CompletedTask;
        }

        public string Print()
        {
            return "현재 등록된 명령어를 보여줍니다.";
        }
    }
}
