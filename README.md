# CLISystem
 
# Default Command Module

```C#
defulatModule.AddCmdProcessor<Close>();

defulatModule.Build();

defulatModule.Run();
```

# Server Command Module

```C#
var serverModule = new NetServerModule(50000);

serverModule.AddCmdProcessor<Close>();

serverModule.Build();

serverModule.Run();
```

# Client Commnad Module

```C#
var client = new NetClientModule("127.0.0.1", 50000);

client.Build();

client.Run();
```

# Custom CLI Command

```C#
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
```

```C#
//...
cli.AddCmdProcessor<Close>();
cli.Build();
cli.Run();
```

# Base Command

? - Cmd List
