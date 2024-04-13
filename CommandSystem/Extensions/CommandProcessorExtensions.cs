using CommandSystem.Interface;

namespace CommandSystem.Extensions
{
    public static class CommandProcessorExtensions
    {
        public static void DisplayPrompt(this ICommandProcessor commandProcessor)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{commandProcessor.GetModuleName()} > ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                _ = Task.Run(commandProcessor.DisplayPrompt);
                return;
            }
            else if (string.IsNullOrEmpty(line.Trim()))
            {
                _ = Task.Run(commandProcessor.DisplayPrompt);
                return;
            }
            commandProcessor.RunCommand(line);
        }
        public static void DisplayPromptOnly(this ICommandProcessor commandProcessor)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{commandProcessor.GetModuleName()} > ");
        }



        //public static void AddCommand<T>(this CommandProcessor commandProcessor,
        //    T command,
        //    bool isLocalCommand = false) where T : class, ICommandAction
        //{
        //    commandProcessor.AddCommandAction(command, isLocalCommand);
        //}

        //public static void AddCommand<T>(this CommandProcessor commandProcessor) where T : class, ICommandAction
        //{
        //    commandProcessor.AddCommandAction<T>();
        //}
        //public static void AddCommand(this CommandProcessor commandProcessor,
        //    string command,
        //    string desc,
        //    Func<string[], CancellationToken, Task> action,
        //    bool isLocalCommand = false)
        //{
        //    commandProcessor.AddCommandAction(command, new ActionCmd(action, desc), isLocalCommand);
        //}
    }
}
