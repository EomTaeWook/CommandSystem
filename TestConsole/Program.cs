// See https://aka.ms/new-console-template for more information
using CommandSystem;
using CommandSystem.Attribude;
using CommandSystem.Interface;
using Dignus.Extensions.Log;
using Dignus.Log;
using System.Diagnostics;


LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}DignusLog.config"));
LogBuilder.Build();

var module = new ServerCmdModule(50000);

module.AddCmdProcessor<Close>();

module.AddCmdProcessor("l", "loop desc", TestAsync);

module.Build();

Task.Run(() =>
{
    module.Run();
});

while (true)
{
    await Task.Delay(33);
}

async Task TestAsync(string[] args, CancellationToken cancellationToken)
{
    var count = 0;
    while (cancellationToken.IsCancellationRequested == false)
    {
        Console.WriteLine($"sleep : {count++}");
        await Task.Delay(1000, cancellationToken);
    }
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





