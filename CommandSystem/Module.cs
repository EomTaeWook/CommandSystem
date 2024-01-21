using CommandSystem.Cmd;
using CommandSystem.Interface;
using CommandSystem.Models;
using System.Reflection;

namespace CommandSystem
{
    public abstract class Module
    {
        internal Builder _builder = new();
        private Configuration _configuration = new();
        private bool _isBuilt = false;
        protected abstract void RunCommand(string line);

        public abstract void Run();

        public Module(string moduleName = null)
        {
            if (string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _configuration.ModuleName = moduleName;
        }

        public void AddCmdProcessor<T>(T processor, bool isLocalCommand = false) where T : class, ICmdProcessor
        {
            _builder.AddProcessorType(processor, isLocalCommand);
        }
        public void AddCmdProcessor<T>() where T : class, ICmdProcessor
        {
            _builder.AddProcessorType<T>();
        }
        public void AddCmdProcessor(string command, string desc, Func<string[], CancellationToken, Task> action, bool isLocalCommand = false)
        {
            _builder.AddProcessorType(command, new ActionCmd(action, desc), isLocalCommand);
        }
        public void Build(Configuration configuration = null)
        {
            if (_isBuilt == true)
            {
                throw new InvalidOperationException("command module has already been built.");
            }
            _isBuilt = true;

            if (configuration != null)
            {
                _configuration = configuration;
            }

            _builder.Build(_configuration);
        }
        public void Prompt()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{_configuration.ModuleName} > ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Task.Run(Prompt);
                return;
            }
            RunCommand(line);
        }
    }
}
