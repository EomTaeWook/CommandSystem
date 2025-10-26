using CommandSystem.Net;
using CommandSystem.Net.Middlewares;
using Dignus.Log;

namespace CommandSystem.Attributes
{
    internal class AuthorizationAttribute : ActionAttribute
    {
        public override bool ActionExecute(CSPipeContext context)
        {
            if (context.Session == null)
            {
                LogHelper.Error($"session is null");
                return false;
            }

            foreach (var item in context.Session.GetSessionComponents())
            {
                if (item is SessionContext sessionContext)
                {
                    return sessionContext.IsAuth;
                }
            }

            return false;
        }
    }
}
