using System;

namespace Tasks
{
    public class CommandLine : ICommandLine
    {
        public string Command { get; }
        public string[] Args { get; }

        public CommandLine(string command)
        {
            var arr = command.Split(" ".ToCharArray(), 2);
            Command = arr[0];
            Args = arr.Length == 2
                ? arr[1].Split(" ")
                : Array.Empty<string>();
        }
    }
}
