# CommandSystem

![picture](/Resource/Output.gif)

# Default Command Module

```C#
defulatModule.AddCmdProcessor<Close>();

defulatModule.Build();

defulatModule.Run();
```

# Server Command Module

```C#
var serverModule = new ServerCmdModule(50000);

serverModule.AddCmdProcessor<Close>();

serverModule.Build();

serverModule.Run();
```

# Client Command Module

```C#
var client = new ClientCmdModule("127.0.0.1", 50000);

client.Build();

client.Run();
```

# Local Command Module

```C#
var local = new LocalCmdModule();

local.Build();

local.Run();
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

Ctrl + C - Command Stop