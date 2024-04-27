using CommandSystem.Extensions;
using CommandSystem.Internals;
using CommandSystem.Net;

namespace CommandSystem
{
    public sealed class ServerCmdModule : CommandProcessorBase
    {
        private readonly ServerModule _serverModule;
        private readonly int _port;
        private readonly JobManager _jobManager;
        private readonly LocalCmdModule _localCmdModule;
        internal JobManager JobManager => _jobManager;

        public ServerCmdModule(int port, string moduleName = null)
        {
            _serverModule = new ServerModule(this);
            _port = port;
            _jobManager = new JobManager();
            _localCmdModule = new LocalCmdModule(moduleName, this._commandServiceContainer);
        }
        public void CancelCommand(int jobId)
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
                await _localCmdModule.RunCommandAsync(jobTask.CommandLine,
                    false,
                    jobTask.CancellationTokenSource.Token);

                _jobManager.CompleteJob();
                _localCmdModule.DisplayPromptOnly();
            }
        }
        public void Run()
        {
            this.RunAsync().GetAwaiter().GetResult();
        }
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                _jobManager.Start();
                _serverModule.Run(_port);
                _ = ProcessCommandAsync();
                _localCmdModule.Run();
            });
        }

        public override void RunCommand(string line)
        {
            _localCmdModule.RunCommand(line);
        }
    }
}
