using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;

namespace CommandSystem.Interfaces
{
    internal interface ITelnetServerEventHandler
    {
        void OnAccepted(IActorRef connectedActorRef);
        void OnDisconnected(IActorRef connectedActorRef);
        void OnDeadLetterMessage(DeadLetterMessage deadLetterMessage);
    }
}
