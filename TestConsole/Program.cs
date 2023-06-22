// See https://aka.ms/new-console-template for more information
using CommandSystem;
using CommandSystem.Attribude;
using CommandSystem.Cmd;
using CommandSystem.Interface;
using Dignus.Extensions.Log;
using Dignus.Log;
using System.Diagnostics;


LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}DignusLog.config"));
LogBuilder.Build();

var module = new NetServerModule(50000);

module.AddCmdProcessor<Close>();

module.AddCmdProcessor("loop", "loop desc", TestAsync);

module.Build();

Task.Run(() =>
{
    module.Run();
});





//var client = new NetClientModule("127.0.0.1", 50000);

//client.Build();

//client.Run();


//module.Run("127.0.0.1", 31000);


while (true)
{
    Thread.Sleep(33);
}

async Task TestAsync(string[] args, CancellationToken cancellationToken)
{
    var count = 0;
    Console.WriteLine($"sleep : {count++}");
    await Task.Delay(10000, cancellationToken);
    Console.WriteLine($"end sleep : {count++}");
}

[CmdAttribute("close")]
internal class Close : ICmdProcessor
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





