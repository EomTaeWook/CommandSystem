using CommandSystem.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Cmd
{

    public class ActionCmd : ICmdProcessor
    {
        private Action<string[]> _action;
        public string _desc;
        public ActionCmd(Action<string[]> action, string desc) 
        {
            _desc = desc;
            _action = action;
        }

        public Task InvokeAsync(string[] args)
        {
            _action.Invoke(args);
            return Task.CompletedTask;
        }

        public string Print()
        {
            return _desc;
        }
    }
}
