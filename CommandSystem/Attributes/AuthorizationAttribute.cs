using CommandSystem.Net.Handler;

namespace CommandSystem.Attributes
{
    internal class AuthorizationAttribute : ActionAttribute
    {
        public override bool ActionExecute(IProtocolHandlerContext context)
        {
            if (context.GetSessionContext() == null)
            {
                return false;
            }
            return true;
        }
    }
}
