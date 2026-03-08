using CommandSystem.Actors;
using CommandSystem.Internals;
using CommandSystem.Messages;
using CommandSystem.Pipeline;
using Dignus.Actor.Core.Actors;
using Dignus.Framework.Pipeline;
using Dignus.Framework.Pipeline.Interfaces;

namespace CommandSystem
{
    public class LocalCommandRunner : CommandModuleBase
    {
        private IActorRef _localConsoleActorRef;

        public event Action ExitRequested;

        private readonly AsyncPipeline<CommandPipelineContext> _commandPipeline = new();

        public LocalCommandRunner(string moduleName = null) : base(moduleName)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        public void Build()
        {
            BuildInternal();

            _commandPipeline.Use(new CommandExecutionMiddleware());

            var executionActorRef = CommandActorSystem.Instance.Spawn(() =>
            {
                return new CommandExecutionActor(_serviceProvider,
                    _commandPipeline,
                    RequestExit);
            });

            _localConsoleActorRef = CommandActorSystem.Instance.Spawn(() =>
            {
                return new LocalConsoleActor(executionActorRef, GetModuleName());
            });
        }
        public void AddMiddleware(IAsyncMiddleware<CommandPipelineContext> middlewareInstance)
        {
            _commandPipeline.Use(middlewareInstance);
        }
        public void AddMiddleware(AsyncPipelineDelegate<CommandPipelineContext> middleware)
        {
            _commandPipeline.Use(middleware);
        }
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            var message = new CancelCommandMessage();
            _localConsoleActorRef.Post(message);
        }
        internal void RequestExit()
        {
            ExitRequested?.Invoke();
        }
        public void Run()
        {
            _localConsoleActorRef.Post(new StartPromptMessage());
        }
    }
}
