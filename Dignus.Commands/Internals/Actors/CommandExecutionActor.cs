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
                _commandMessages.Add(runCommandMessage);

                if (_cancellationToken != null)
                {
                    return;
                }

                if (_commandMessages.TryRead(out var item))
                {
                    await RunCommandAsync(item.CommandLine, sender);
                }
            }
            else if(message is CancelCommandMessage)
            {
                if(_cancellationToken != null)
                {
                    _cancellationToken.Cancel();
                }
                else
                {
                    onExitRequested?.Invoke();
                }
            }
            else if(message is CompleteCommandMessage)
            {
                _cancellationToken.Dispose();

                _cancellationToken = null;

                if (_commandMessages.TryRead(out var item))
                {
                    await RunCommandAsync(item.CommandLine, sender);
                }
            }
        }
        public async Task RunCommandAsync(string line, IActorRef actorRef)
        {
            if (_cancellationToken != null)
            {
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            await RunCommandAsync(line, false, actorRef, _cancellationToken.Token);
        }
        public Task RunCommandAsync(string line, bool isAlias, IActorRef actorRef, CancellationToken cancellationToken)
        {
            var splits = line.Split(" ");
            _ = ExecuteCommandInternalAsync(splits[0], splits[1..], isAlias, actorRef, cancellationToken);
            return Task.CompletedTask;
        }

        private async Task ExecuteCommandInternalAsync(string commandName,
            string[] args,
            bool isAlias,
            IActorRef sender,
            CancellationToken cancellationToken)
        {
            var aliasTable = serviceProvider.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(commandName) == true && isAlias == false)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[commandName].Cmd);
                sb.Append(string.Join(" ", args));

                await RunCommandAsync(sb.ToString(), true, sender, cancellationToken);
            }

            var commandTable = serviceProvider.GetService<CommandTable>();
            var commandType = commandTable.GetCommandType(commandName);

            if (commandType == null)
            {
                sender.Post(new CommandResponseMessage() 
                {
                    Content = $"Command `{commandName}` was not found. Please type 'help' to see the available commands."
                }, Self);

                sender.Post(new CompleteCommandMessage(), Self);
                return;
            }

            try
            {
                var command = (ICommand)serviceProvider.GetService(commandType);
                var context = new CommandPipelineContext()
                {
                    CancellationToken = cancellationToken,
                    Command = command,
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
