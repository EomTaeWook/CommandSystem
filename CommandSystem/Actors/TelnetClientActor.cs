using CommandSystem.Internals;
using CommandSystem.Messages;
using CommandSystem.Network.Decoder;
using CommandSystem.Network.Messages;
using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Actors;
using System.Text;

namespace CommandSystem.Actors
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

        private readonly TelnetProtocolDecoder _protocolDecoder = new();
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if(message is CommandLineMessage commandLineMessage)
            {
                HandleInput(commandLineMessage);
                return ValueTask.CompletedTask;
            }
            else if(message is CancelCommandMessage)
            {
                commandExecutionActorRef.Post(message);
            }
            else if(message is CompleteCommandMessage)
            {
                commandExecutionActorRef.Post(message);
                ShowPrompt();
            }
            else if(message is RawNetworkMessage networkMessage)
            {
                NetworkSession.SendAsync(networkMessage.Bytes);
            }
            else if(message is CommandResponseMessage commandResponse)
            {
                var bytes = Encoding.UTF8.GetBytes($"\r\n{commandResponse.Content}");
                NetworkSession.SendAsync(bytes);
            }

            return ValueTask.CompletedTask;
        }
        private void ShowPrompt() 
        {
            var bytes = Encoding.UTF8.GetBytes($"\r\n{moduleName} > ");
            var promptMessage = new RawNetworkMessage()
            {
                Bytes = bytes
            };
            Self.Post(promptMessage);
        }

        private void HandleInput(CommandLineMessage message)
        {
            _protocolDecoder.DecodeIncomingNetworkBytes(message.CommandLine,
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
                            commandExecutionActorRef.Post(new RunCommandMessage(commandLine), Self);
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
