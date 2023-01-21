using CLISystem.Interface;
using Kosher.Log;

namespace CLISystem
{
    public class CLIModule
    {
        private readonly Builder builder = new();
        private readonly Dictionary<string, ICmdProcessor> _cmdProcessor = new();
        private bool _isBuild = false;

        private bool RunCommnad(string command, string[] args)
        {
            if (_cmdProcessor.ContainsKey(command) == false)
            {
                return false;
            }
            var cmdProcessor = _cmdProcessor[command];

            cmdProcessor.Invoke(args);

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
                LogHelper.Debug($"cli module start...");
                var option = new List<string>();

                while (true)
                {
                    var line = Console.ReadLine();
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    var splits = line.Split(" ");

                    for (int i = 1; i < splits.Length; ++i)
                    {
                        option.Add(splits[i]);
                    }

                    var excuted = RunCommnad(splits[0], splits[1..]);
                    if (excuted == false)
                    {
                        LogHelper.Error($"faild to command {splits[0]}");
                    }

                    option.Clear();
                }
            });
        }
    }
}
