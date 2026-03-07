using CommandSystem.Actors;
using CommandSystem.Internals;
using CommandSystem.Messages;
using Dignus.Actor.Core.Actors;

namespace CommandSystem
{
    public class LocalCommandRunner : CommandModuleBase
    {
        private IActorRef _localConsoleActorRef;

        public event Action ExitRequested;
        public LocalCommandRunner(string moduleName = null) : base(moduleName)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        public void Build()
        {
            BuildInternal();
            
            var executionActorRef = CommandActorSystem.Instance.Spawn(() =>
            {
                return new CommandExecutionActor(_serviceProvider, RequestExit);
            });

            _localConsoleActorRef = CommandActorSystem.Instance.Spawn(() =>
            {
                return new LocalConsoleActor(executionActorRef, GetModuleName());
            });
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
