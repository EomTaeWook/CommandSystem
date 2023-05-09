using System.Text;

namespace CommandSystem.ConsoleWriter
{
    public class RedirectConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        readonly TextWriter _oldWriter;
        readonly StringBuilder sb = new StringBuilder();
        public RedirectConsoleWriter()
        {
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
            sb.Append(line);
        }
        public string Release()
        {
            return sb.ToString();
        }
    }
}
