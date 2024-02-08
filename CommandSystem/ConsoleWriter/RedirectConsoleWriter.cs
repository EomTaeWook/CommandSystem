using Dignus.DependencyInjection.Attribute;
using System.Text;

namespace CommandSystem.ConsoleWriter
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class RedirectConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        readonly TextWriter _oldWriter;
        readonly StringBuilder sb = new();
        public RedirectConsoleWriter()
        {
            _oldWriter = Console.Out;
            Console.SetOut(this);
        }
        public override void Write(char[] buffer)
        {
            _oldWriter.Write(buffer);
            sb.Append(buffer);
        }
        public override void Write(char value)
        {
            _oldWriter.Write(value);
            sb.Append(value);
        }
        public override void WriteLine(string line)
        {
            _oldWriter.WriteLine(line);
            sb.AppendLine(line);
        }
        public string Release()
        {
            var text = sb.ToString();
            sb.Clear();
            return text;
        }
    }
}
