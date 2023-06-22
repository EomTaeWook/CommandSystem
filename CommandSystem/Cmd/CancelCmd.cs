using CommandSystem.Attribude;
using CommandSystem.Interface;

namespace CommandSystem.Cmd
{
    [Cmd("cancel")]
    internal class CancelCmd : ICmdProcessor
    {
        public CancelCmd()
        {
        }
        public Task InvokeAsync(string[] args, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public string Print()
        {
            return "실행중인 명령을 중지합니다.";
        }
    }
}
