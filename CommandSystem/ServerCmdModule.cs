using CommandSystem.Internal;
using CommandSystem.Net;

namespace CommandSystem
{
    public sealed class ServerCmdModule : LocalCmdModule
    {
        private readonly ServerModule _serverModule;
        private readonly int _port;
        private JobManager _jobManager;

        internal JobManager JobManager => _jobManager;
        public ServerCmdModule(int port, string moduleName = null) : base(moduleName)
        {
            _serverModule = new ServerModule(this);
            _port = port;
            _jobManager = new JobManager();
        }
        public void CacelCommand(int jobId)
        {
            _jobManager.RemoveJob(jobId);
        }

        public async Task ProcessCommandAsync()
        {
            while (true)
            {
                var jobTask = _jobManager.DequeueNextJob();
                if (jobTask == null)
                {
                    await Task.Delay(100);
                    continue;
                }
                Console.WriteLine();
                await RunCommand(jobTask.CommandLine,
                    false,
                    jobTask.CancellationTokenSource.Token);

                _jobManager.CompleteJob();
                DisplayPromptOnly();
            }
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                _jobManager.Start();
                _serverModule.Run(_port, _builder);
                _ = ProcessCommandAsync();
                base.Run();
            });
        }
    }
}
