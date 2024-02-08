using CommandSystem.ConsoleWriter;
using CommandSystem.Models;
using CommandSystem.Net;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using Dignus.Collections;
using Dignus.Coroutine;
using Dignus.DependencyInjection.Attribute;
using Dignus.Sockets;
using System.Collections;

namespace CommandSystem.Internal
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    internal class JobManager
    {
        private readonly SynchronizedArrayQueue<CommandExecutionTask> _jobQueue = new();
        private int _jobIdCounter;
        private readonly RedirectConsoleWriter _redirectConsoleWriter;
        private CommandExecutionTask _currentTask;
        private readonly CoroutineHandler _coroutineHandler = new();
        private bool _running;
        public JobManager()
        {
            _redirectConsoleWriter = new RedirectConsoleWriter();
        }
        [InjectConstructor]
        public JobManager(RedirectConsoleWriter redirectConsoleWriter)
        {
            _redirectConsoleWriter = redirectConsoleWriter;
        }
        public Task Start()
        {
            if (_running)
            {
                return Task.CompletedTask;
            }

            _running = true;
            var task = Task.Run(async () =>
            {
                while (_running)
                {
                    await Task.Delay(60);
                    _coroutineHandler.UpdateCoroutines(0.060F);
                }
            });
            return task;
        }

        private IEnumerator SendConsoleOutput(CommandExecutionTask commandExecutionTask)
        {
            while (commandExecutionTask.JobId != -1)
            {
                var body = _redirectConsoleWriter.Release();
                var packet = Packet.MakePacket((int)SCProtocol.NotifyConsoleText,
                    new NotifyConsoleText()
                    {
                        ConsoleText = body
                    });
                commandExecutionTask.Session.Send(packet);
                yield return null;
            }
        }

        private int GenerateNewJobId()
        {
            Interlocked.Increment(ref _jobIdCounter);
            return _jobIdCounter;
        }
        public bool RemoveJob(int jobId)
        {
            if (_currentTask.JobId == jobId)
            {
                _currentTask.CancellationTokenSource.Cancel();
                var body = _redirectConsoleWriter.Release();
                var packet = Packet.MakePacket((int)SCProtocol.NotifyConsoleText,
                    new NotifyConsoleText()
                    {
                        ConsoleText = body
                    });
                _currentTask.Session.Send(packet);
            }

            for (int i = 0; i < _jobQueue.Count; ++i)
            {
                if (_jobQueue[i].JobId == jobId)
                {
                    _jobQueue[i] = null;
                    return true;
                }
            }
            return false;
        }
        public void CompleteJob()
        {
            var body = _redirectConsoleWriter.Release();
            var packet = Packet.MakePacket((int)SCProtocol.CompleteRemoteCommand,
                    new CompleteRemoteCommand()
                    {
                        ConsoleText = body,
                        JobId = _currentTask.JobId
                    });
            _currentTask.Session.Send(packet);
            _currentTask.Complete();
        }
        public int AddJob(string line, Session session)
        {
            var id = GenerateNewJobId();
            var jobTask = new CommandExecutionTask()
            {
                Session = session,
                CommandLine = line,
                JobId = id,
                CancellationTokenSource = new CancellationTokenSource()
            };
            _jobQueue.Add(jobTask);
            return id;
        }

        internal CommandExecutionTask DequeueNextJob()
        {
            while (_jobQueue.CanRead)
            {
                _currentTask = _jobQueue.Read();
                if (_currentTask != null)
                {
                    _coroutineHandler.Start(SendConsoleOutput(_currentTask));
                    return _currentTask;
                }
            }
            return null;
        }
    }
}
