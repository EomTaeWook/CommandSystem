namespace CLISystem.Net.Protocol.Handler
{
    internal interface IProtocolHandler
    {
        void Process(int protocol, string body);
    }
}
