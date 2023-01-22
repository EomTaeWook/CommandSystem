using CLISystem.Interface;
using CLISystem.Models;
using Kosher.Log;
using System.Text;

namespace CLISystem
{
    public class CLIModule
    {
        private readonly Builder builder = new();
        private readonly Dictionary<string, ICmdProcessor> _cmdProcessor = new();
        private bool _isBuild = false;

        private bool RunCommnad(string line)
        {
            var splits = line.Split(" ");

            var excuted = RunCommnad(splits[0], splits[1..]);
            if (excuted == false)
            {
                LogHelper.Error($"faild to command {splits[0]}");
            }

            return true;
        }
        private bool RunCommnad(string command, string[] options)
        {
            var aliasTable = builder.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(command) == true)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[command].Cmd);
                sb.Append(String.Join(" ", options));

                return RunCommnad(sb.ToString());
            }

            if (_cmdProcessor.ContainsKey(command) == false)
            {
                return false;
            }
            
            var cmdProcessor = builder.GetService<ICmdProcessor>(command);
            cmdProcessor.Invoke(options);

            return true;
        }
        public void AddCmdProcessor<T>(T processor) where T : class, ICmdProcessor
        {
            builder.AddProcessorType(processor);
        }
        public void AddCmdProcessor<T>() where T : class, ICmdProcessor
        {
            builder.AddProcessorType<T>();
        }
        public void Build()
        {
            if(_isBuild == true)
            {
                throw new InvalidOperationException("already build cli module");
            }
            _isBuild = true;
            builder.AddSingletonService(_cmdProcessor);
            builder.Build();

            foreach(var item in builder._processTypeNames)
            {
                var processor = builder.GetService<ICmdProcessor>(item);
                _cmdProcessor.Add(item, processor);
            }
        }
        
        public void Run()
        {
            Task.Run(() => 
            {
                Console.WriteLine($"*** cli module start ***");

                while (true)
                {
                    var line = Console.ReadLine();
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    RunCommnad(line);
                }
            });
        }
    }
}
