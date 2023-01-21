# CLISystem
 
CLIModule cli = new CLIModule();
cli.Build();
cli.Run();

# Custom CLI Command

```C#
[CmdAttribude("close")]
internal class Close : ICmdProcessor
{
    public void Invoke(string[] args)
    {
        Process.GetCurrentProcess().Close();
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