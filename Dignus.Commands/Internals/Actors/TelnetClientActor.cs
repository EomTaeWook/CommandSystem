using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Actors;
using Dignus.Commands.Messages;
using Dignus.Commands.Network.Decoders;
using Dignus.Commands.Network.Messages;
using System.Text;

namespace Dignus.Commands.Internals.Actors
{
    internal class TelnetClientActor(IActorRef commandExecutionActorRef,
        string moduleName) : SessionActorBase
    {
        private static readonly byte[] BackspaceEraseSequence =
        [
            (byte)ControlCharacter.Backspace,
            0x20,
            (byte)ControlCharacter.Backspace
        ];

        private readonly List<string> _currentCommandPath = [];
        private readonly TelnetProtocolDecoder _protocolDecoder = new();
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if(message is IncomingNetworkMessage commandLineMessage)
            {
                HandleInput(commandLineMessage);
                return ValueTask.CompletedTask;
            }
            else if(message is CancelCommandMessage)
            {
                commandExecutionActorRef.Post(message, Self);
            }
            else if(message is CompleteCommandMessage)
            {
                commandExecutionActorRef.Post(message, Self);
                ShowPrompt();
            }
            else if(message is OutgoingNetworkMessage networkMessage)
            {
                NetworkSession.SendAsync(networkMessage.Bytes);
            }
            else if(message is CommandResponseMessage commandResponse)
            {
                var bytes = Encoding.UTF8.GetBytes($"\r\n{commandResponse.Content}");
                NetworkSession.SendAsync(bytes);
            }
            else if(message is ChangeDirectoryRequestMessage changeDirectoryRequestMessage)
            {
                HandleDirectoryChanged(changeDirectoryRequestMessage);
            }

            return ValueTask.CompletedTask;
        }
        private void ShowPrompt() 
        {
            var currentPath = "/";
            if(_currentCommandPath.Count > 0)
            {
                currentPath = string.Join("/", _currentCommandPath);
            }

            var bytes = Encoding.UTF8.GetBytes($"{moduleName}:{currentPath} > ");
            var promptMessage = new OutgoingNetworkMessage()
            {
                Bytes = bytes
            };
            Self.Post(promptMessage);
        }
        private void HandleDirectoryChanged(ChangeDirectoryRequestMessage changeDirectoryRequest)
        {
            var result = CommandPathResolver.Resolve(_currentCommandPath, changeDirectoryRequest.Path);
            _currentCommandPath.Clear();
            _currentCommandPath.AddRange(result);
        }
        private void HandleInput(IncomingNetworkMessage message)
        {
            _protocolDecoder.DecodeIncomingNetworkBytes(message.Bytes,
                HandleValidCharacter);
        }

        private void HandleValidCharacter(char character)
        {
            ControlCharacter controlCharacter = (ControlCharacter)character;

            switch (controlCharacter)
            {
                case ControlCharacter.Backspace:
                case ControlCharacter.Delete:
                    {
                        if (_protocolDecoder.IsBufferEmpty == false)
                        {
                            _protocolDecoder.RemoveLastCharacterFromBuffer();
                            NetworkSession.SendAsync(BackspaceEraseSequence);
                        }

                        return;
                    }

                case ControlCharacter.CarriageReturn:
                    return;

                case ControlCharacter.LineFeed:
                    {
                        string commandLine = _protocolDecoder.GetFinalCommandAndClearBuffer();

                        if (string.IsNullOrWhiteSpace(commandLine) == false)
                        {
                            commandExecutionActorRef.Post(new RunCommandMessage(string.Join("/", _currentCommandPath), commandLine), Self);
                        }
                        return;
                    }

                case ControlCharacter.EndOfText:
                    {
                        commandExecutionActorRef.Post(new CancelCommandMessage(), Self);
                        return;
                    }
            }

            _protocolDecoder.AppendCharacterToBuffer(character);
            NetworkSession.SendAsync([(byte)character]);
        }
    }
}
