using CommandSystem.Net;

namespace CommandSystem.Attributes
{
    internal class AuthorizationAttribute : ActionAttribute
    {
        public override bool ActionExecute(PipeContext context)
        {
            if (context.Session == null)
            {
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
