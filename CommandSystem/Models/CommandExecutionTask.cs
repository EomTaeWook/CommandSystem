using Dignus.Sockets.Interfaces;

namespace CommandSystem.Models
{
    internal class CommandExecutionTask
    {
        public int JobId { get; set; }

        public ISession Session { get; set; }

        public string CommandLine { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void Complete()
        {
            JobId = -1;
        }
    }
}
