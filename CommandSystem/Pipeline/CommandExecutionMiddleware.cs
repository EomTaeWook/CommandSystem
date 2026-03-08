using Dignus.Framework.Pipeline;
using Dignus.Framework.Pipeline.Interfaces;

namespace CommandSystem.Pipeline
{
    internal class CommandExecutionMiddleware : IAsyncMiddleware<CommandPipelineContext>
    {
        public Task InvokeAsync(ref CommandPipelineContext context,
            ref AsyncPipelineNext<CommandPipelineContext> next)
        {
            var command = context.Command;
            return command.InvokeAsync(context.CommandArguments,
                context.SenderActorRef,
                context.CancellationToken);
        }
    }
}
