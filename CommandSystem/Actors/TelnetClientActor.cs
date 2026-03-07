using CommandSystem.Internals;
using CommandSystem.Messages;
using CommandSystem.Network.Messages;
using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Actors;
using System.Text;

namespace CommandSystem.Actors
{
    internal class TelnetClientActor(IActorRef commandExecutionActorRef, string moduleName) : SessionActorBase
    {
        private readonly StringBuilder _inputBuffer = new();
        private bool _isHandlingTelnetCommand = false;
        private bool _isHandlingEscapeSequence = false;
        protected override ValueTask OnReceive(IActorMessage message, IActorRef sender)
        {
            if(message is CommandLineMessage commandLineMessage)
            {
                HandleInput(commandLineMessage);
                return ValueTask.CompletedTask;
            }
            else if(message is CompleteCommandMessage)
            {
                ShowPrompt();
                commandExecutionActorRef.Post(message);
            }
            else if(message is RawNetworkMessage networkMessage)
            {
                NetworkSession.SendAsync(networkMessage.Bytes);
            }
            else if(message is CommandResponseMessage commandResponse)
            {
                var bytes = Encoding.GetEncoding(949).GetBytes($"\r\n{commandResponse.Content}\r\n");
                NetworkSession.SendAsync(bytes);
            }

            return ValueTask.CompletedTask;
        }
        private void ShowPrompt() 
        {
            var bytes = Encoding.GetEncoding(949).GetBytes($"{moduleName} > ");
            var promptMessage = new RawNetworkMessage()
            {
                Bytes = bytes
            };
            Self.Post(promptMessage);
        }

        private void HandleInput(CommandLineMessage message)
        {
            for (int i = 0; i < message.CommandLine.Count; ++i)
            {
                byte currentByte = message.CommandLine[i];

                // Telnet IAC 협상 필터
                if (currentByte == 0xFF || _isHandlingTelnetCommand)
                {
                    _isHandlingTelnetCommand = true;
                    if (currentByte != 0xFF && currentByte <= 0xEF)
                    {
                        _isHandlingTelnetCommand = false;
                    }
                    continue;
                }

                // ANSI escape sequence 필터
                if (_isHandlingEscapeSequence)
                {
                    if (currentByte >= 0x40 && currentByte <= 0x7E)
                    {
                        _isHandlingEscapeSequence = false;
                    }

                    continue;
                }
                ControlCharacter controlCharacter = (ControlCharacter)currentByte;

                switch (controlCharacter)
                {
                    case ControlCharacter.Esc:
                        {
                            _isHandlingEscapeSequence = true;
                            continue;
                        }
                        
                    case ControlCharacter.Backspace:
                    case ControlCharacter.Delete:
                        {
                            if (_inputBuffer.Length > 0)
                            {
                                _inputBuffer.Length--;

                                byte[] visualEraseSequence = [0x08, 0x20, 0x08];
                                NetworkSession.SendAsync(visualEraseSequence);
                            }
                            continue;
                        }

                    case ControlCharacter.CarriageReturn:
                        continue;

                    case ControlCharacter.LineFeed:
                        {
                            string commandLine = _inputBuffer.ToString();
                            _inputBuffer.Clear();

                            if (!string.IsNullOrWhiteSpace(commandLine))
                            {
                                commandExecutionActorRef.Post(new RunCommandMessage(commandLine), Self);
                            }
                            continue;
                        }
                    case ControlCharacter.EndOfText:
                        {
                            commandExecutionActorRef.Post(new CancelCommandMessage(), Self);
                            continue;
                        }
                }
                _inputBuffer.Append((char)currentByte);
                NetworkSession.SendAsync([currentByte]);
            }
        }
    }
}
