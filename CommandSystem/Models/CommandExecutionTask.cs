using Dignus.Sockets;

namespace CommandSystem.Models
{
    internal class CommandExecutionTask
    {
        public int JobId { get; set; }

        public Session Session { get; set; }

        public string CommandLine { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public void Complete()
        {
            JobId = -1;
        }
    }
}
