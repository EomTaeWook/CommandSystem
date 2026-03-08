using Dignus.Commands.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTestConsole.Attributes
{
    internal class AuthAttribute : Attribute
    {
        public bool Execute(CommandPipelineContext commandPipelineContext)
        {
            return false;
        }
    }
}
