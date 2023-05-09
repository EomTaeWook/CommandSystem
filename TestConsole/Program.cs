// See https://aka.ms/new-console-template for more information
using CommandSystem;
using CommandSystem.Attribude;
using CommandSystem.Interface;
using Kosher.Extensions.Log;
using Kosher.Log;
using System.Diagnostics;


LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}KosherLog.config"));
LogBuilder.Build();

var module = new NetServerModule(50000);

module.AddCmdProcessor<Close>();

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


[CmdAttribute("close")]
internal class Close : ICmdProcessor
{
    public Task InvokeAsync(string[] args)
    {
        Process.GetCurrentProcess().Close();
        return Task.CompletedTask;
    }

    public string Print()
    {
        return "close process";
    }
}





