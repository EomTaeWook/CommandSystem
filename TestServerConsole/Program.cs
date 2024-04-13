using CommandSystem;
using CommandSystem.Attribute;
using CommandSystem.Interface;
using Dignus.Extensions.Log;
using Dignus.Log;
using System.Diagnostics;

namespace TestConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}DignusLog.config"));
            LogBuilder.Build();

            var client = new ClientCmdModule("127.0.0.1", 50000);
            client.Build();
            client.Run();

            while (true)
            {
                Thread.Sleep(33);
            }
        }
        static async Task TestAsync(string[] args, CancellationToken cancellationToken)
        {
            var count = 0;
            while (cancellationToken.IsCancellationRequested == false)
            {
                Console.WriteLine($"sleep : {count++}");
                await Task.Delay(1000, cancellationToken);
            }
            Console.WriteLine($"end sleep : {count++}");
        }

        [Cmd("close")]
        internal class Close : ICommandAction
        {
            public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
            {
                Process.GetCurrentProcess().Close();
                return Task.CompletedTask;
            }

            public string Print()
            {
                return "close process";
            }
        }
    }

}