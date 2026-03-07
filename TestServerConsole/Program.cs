// See https://aka.ms/new-console-template for more information
using CommandSystem;
using CommandSystem.Attributes;
using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;
using Dignus.Log;
using System.Diagnostics;


LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}DignusLog.config"));
LogBuilder.Build();

var module = new TelnetCommandRunner();

module.AddCommandAction<Close>();

module.AddCommandAction("l", "loop desc", TestAsync);

module.Build();

module.Run();

while (true)
{
    await Task.Delay(33);
}

async Task TestAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
{
    var count = 0;
    while (cancellationToken.IsCancellationRequested == false)
    {
        Console.WriteLine($"sleep : {count++}");
        await Task.Delay(1000, cancellationToken);
    }
    Console.WriteLine($"end sleep : {count++}");
}

[Command("close")]
internal class Close : ICommand
{
    public Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken)
    {
        Process.GetCurrentProcess().Close();
        return Task.CompletedTask;
    }

    public string Print()
    {
        return "close process";
    }
}





