// See https://aka.ms/new-console-template for more information
using CLISystem;
using Kosher.Extensions.Log;
using Kosher.Log;

LogBuilder.Configuration(LogConfigXmlReader.Load($"{AppContext.BaseDirectory}KosherLog.config"));
LogBuilder.Build();

var module = new CLIModule();

module.Build();
module.Run();


