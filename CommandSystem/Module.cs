using CommandSystem.Cmd;
using CommandSystem.Interface;
using CommandSystem.Models;
using System.Reflection;

namespace CommandSystem
{
    public abstract class Module
    {
        internal Builder _builder = new();

        private bool _isBuild = false;
        private readonly Configuration _configuration = new();
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

        public void AddCmdProcessor<T>(T processor) where T : class, ICmdProcessor
        {
            _builder.AddProcessorType(processor);
        }
        public void AddCmdProcessor<T>() where T : class, ICmdProcessor
        {
            _builder.AddProcessorType<T>();
        }
        public void AddCmdProcessor(string command, string desc, Func<string[], CancellationToken, Task> action)
        {
            _builder.AddProcessorType(command, new ActionCmd(action, desc));
        }
        public void Build(Configuration configuration = null)
        {
            if (_isBuild == true)
            {
                throw new InvalidOperationException("already build cli module");
            }
            _isBuild = true;

            if (configuration == null)
            {
                configuration = _configuration;
            }
            _builder.Build(configuration);
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
