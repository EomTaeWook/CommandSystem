using CLISystem.Attribude;
using CLISystem.Interface;
using Kosher.Framework;
using System.Reflection;

namespace CLISystem
{
    internal class Builder
    {
        internal List<string> _processTypeNames = new List<string>();
        readonly ServiceProvider _serviceProvider = new();
        public void Build()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var cmdAttribudeType = typeof(CmdAttribude);

            foreach (var item in assembly.GetTypes())
            {
                if (item.IsDefined(cmdAttribudeType))
                {
                    var attr = item.GetCustomAttribute(cmdAttribudeType) as CmdAttribude;
                    AddNamedCmdType(attr.Name, item);
                    _processTypeNames.Add(attr.Name);
                }
            }
        }
        public void AddProcessorType<T>(T cmdProcessor) where T : class, ICmdProcessor
        {
            var cmdAttribudeType = typeof(CmdAttribude);
            var processorType = cmdProcessor.GetType();
            string name;
            if (processorType.IsDefined(cmdAttribudeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttribudeType) as CmdAttribude;
                name = attr.Name;
            }
            else
            {
                name = processorType.Name;
            }
            AddNamedCmdType(name, cmdProcessor);
            _processTypeNames.Add(name);
        }
        public void AddProcessorType<T>() where T : class, ICmdProcessor
        {
            var cmdAttribudeType = typeof(CmdAttribude);
            var processorType = typeof(T);
            string name;
            if (processorType.IsDefined(cmdAttribudeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttribudeType) as CmdAttribude;
                name = attr.Name;
            }
            else
            {
                name = processorType.Name;
            }
            AddNamedCmdType(name, processorType);
            _processTypeNames.Add(name);
        }

        private void AddNamedCmdType<T>(string typeName, T implementation) where T : class
        {
            _serviceProvider.AddSingleton(typeName, implementation);
        }
        private void AddNamedCmdType(string typeName, Type type)
        {
            _serviceProvider.AddSingleton(typeName, type);
        }
        public void AddSingletonService<T>(T implementation) where T : class
        {
            _serviceProvider.AddSingleton(implementation);
        }
        public T GetService<T>(string typeName) where T : class
        {
            return _serviceProvider.GetService<T>(typeName);
        }
    }
}
