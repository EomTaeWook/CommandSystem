using CLISystem.Models;
using System.Text;

namespace CLISystem.ConsoleWriter
{
    public class ModuleConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        readonly TextWriter _oldWriter;
        readonly Configuration _configuration;
        public ModuleConsoleWriter(Configuration configuration)
        {
            _configuration = configuration;
            _oldWriter = Console.Out;
            Console.SetOut(this);
        }
        public override void Write(char[] buffer)
        {
            var sb = new StringBuilder(new string(buffer));
            WriteLine(sb.ToString());
        }
        public override void WriteLine(string line)
        {
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            _oldWriter.WriteLine(line);
            Console.ForegroundColor = ConsoleColor.White;
            _oldWriter.Write($"{_configuration.ModuleName} > ");
        }
    }
}
