using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Collections;
using Dignus.Commands.Interfaces;
using Dignus.Commands.Messages;
using Dignus.Commands.Pipeline;
using Dignus.DependencyInjection.Extensions;
using Dignus.Framework.Pipeline;
using System.Text;

namespace Dignus.Commands.Internals.Actors
{
    internal class CommandExecutionActor(IServiceProvider serviceProvider,
        AsyncPipeline<CommandPipelineContext> commandPipeline,
        Action onExitRequested
        ) : ActorBase
    {
        private CancellationTokenSource _cancellationToken;
        private readonly ArrayQueue<RunCommandMessage> _commandMessages = [];
        protected override async ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if (message is RunCommandMessage runCommandMessage)
            {
                await HandleRunCommandAsync(runCommandMessage, sender);
            }
            else if(message is CancelCommandMessage)
            {
                await HandleCancelCommandAsync();
            }
            else if(message is CompleteCommandMessage)
            {
                await HandleCompleteCommandAsync(sender);
            }

        }
        private async Task HandleCompleteCommandAsync(IActorRef sender)
        {
            _cancellationToken?.Dispose();

            _cancellationToken = null;

            if (_commandMessages.TryRead(out var item))
            {
                await RunCommandAsync(item.CurrentPath, item.CommandLine, sender);
            }
        }
        private Task HandleCancelCommandAsync()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
            else
            {
                onExitRequested?.Invoke();
            }
            return Task.CompletedTask;
        }

        private async Task HandleRunCommandAsync(RunCommandMessage runCommandMessage, IActorRef sender)
        {
            _commandMessages.Add(runCommandMessage);

            if (_cancellationToken != null)
            {
                return;
            }

            if (_commandMessages.TryRead(out var item))
            {
                await RunCommandAsync(item.CurrentPath, item.CommandLine, sender);
            }
        }
        private async Task RunCommandAsync(string currentPath, string commandLine, IActorRef actorRef)
        {
            if (_cancellationToken != null)
            {
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            await RunCommandAsync(currentPath, commandLine, false, actorRef, _cancellationToken.Token);
        }
        private Task RunCommandAsync(string currentPath, string commandLine, bool isAlias, IActorRef actorRef, CancellationToken cancellationToken)
        {
            var splits = commandLine.Split(" ");
            _ = ExecuteCommandInternalAsync(currentPath, splits[0], splits[1..], isAlias, actorRef, cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ExecuteCommandInternalAsync(
            string currentPath,
            string commandName,
            string[] args,
            bool isAlias,
            IActorRef sender,
            CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(commandName))
            {
                sender.Post(new CompleteCommandMessage(), Self);
                return;
            }

            if(currentPath.StartsWith('/'))
            {
                currentPath = currentPath.TrimStart('/');
            }

            var aliasTable = serviceProvider.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(commandName) == true && isAlias == false)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[commandName].Cmd);
                sb.Append(string.Join(" ", args));

                await RunCommandAsync(currentPath, sb.ToString(), true, sender, cancellationToken);
            }

            var commandTable = serviceProvider.GetService<CommandTable>();
            var commandType = commandTable.GetCommandType(currentPath, commandName);

            if (commandType == null)
            {
                commandType = commandTable.GetGlobalCommandType(commandName);

                if(commandType == null)
                {
                    sender.Post(new CommandResponseMessage()
                    {
                        Content = $"Command `{commandName}` was not found. Please type 'help' to see the available commands."
                    }, Self);

                    sender.Post(new CompleteCommandMessage(), Self);
                    return;
                }
            }

            try
            {
                var command = (IPathCommand)serviceProvider.GetService(commandType);
                var context = new CommandPipelineContext()
                {
                    CancellationToken = cancellationToken,
                    Command = command,
                    CurrentPath = currentPath,
                    CommandArguments = args,
                    SenderActorRef = sender
                };

               await commandPipeline.InvokeAsync(ref context);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sender.Post(new CompleteCommandMessage(), Self);
            }
        }
    }
}
