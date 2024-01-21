using CommandSystem.Interface;

namespace CommandSystem.Cmd
{
    internal class ActionCmd : ICmdProcessor
    {
        private readonly Func<string[], CancellationToken, Task> _func;
        public string _desc;
        public ActionCmd(Func<string[], CancellationToken, Task> func, string desc)
        {
            _desc = desc;
            _func = func;
        }
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            return _func.Invoke(args, cancellationToken);
        }

        public string Print()
        {
            return _desc;
        }
    }
}
