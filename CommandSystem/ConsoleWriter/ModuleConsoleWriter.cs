using CommandSystem.Models;
using System.Text;

namespace CommandSystem.ConsoleWriter
{
    public class ModuleConsoleWriter : TextWriter
    {
        private bool _isPrompt = false;
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
            if (_isPrompt == true)
            {
                ClearLine();
                _isPrompt = false;
            }
            _oldWriter.WriteLine(line);
            Prompt();
        }
        public void ClearLine()
        {
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        }
        public void Prompt()
        {
            ClearLine();
            _isPrompt = true;
            Console.ForegroundColor = ConsoleColor.White;
            _oldWriter.Write($"{_configuration.ModuleName} > ");
        }
    }
}
