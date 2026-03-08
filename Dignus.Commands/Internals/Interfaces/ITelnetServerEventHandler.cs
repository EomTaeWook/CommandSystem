using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Commands.Internals.Actors;

namespace Dignus.Commands.Internals.Interfaces
{
    internal interface ITelnetServerEventHandler
    {
        TelnetClientActor CreateSessionActor();
        void OnAccepted(IActorRef connectedActorRef);
        void OnDisconnected(IActorRef connectedActorRef);
        void OnDeadLetterMessage(DeadLetterMessage deadLetterMessage);
    }
}
