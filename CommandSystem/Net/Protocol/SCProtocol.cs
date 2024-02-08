namespace CommandSystem.Net.Protocol
{
    internal enum SCProtocol : ushort
    {
        GetModuleInfoResponse,
        RemoteCommandResponse,
        CancelCommandResponse,
        NotifyConsoleText,
        CompleteRemoteCommand,
        Max
    }
}
