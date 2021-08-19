using System;

namespace Tasks
{
    public interface ICommandLine
    {
        string Arg { get; }
    }

    public class CommandLine : ICommandLine
    {
        public string Arg { get; }

        public CommandLine(string arg)
        {
            Arg = arg;
        }
    }

    public class AddTaskCommandLine : CommandLine
    {
        public string Project { get; }
        public string Description { get; }

        public AddTaskCommandLine(string arg) : base(arg)
        {
            var args = arg.Split(" ", 4);
            Project = args[2];
            Description = args[3];
        }
    }

    public class AddProjectCommandLine : CommandLine
    {
        public string Project { get; }

        public AddProjectCommandLine(string arg) : base(arg)
        {
            var args = arg.Split(" ", 3);
            Project = args[2];
        }
    }

    public class CheckCommandLine : CommandLine
    {
        public Id Id { get; }

        public CheckCommandLine(string arg) : base(arg)
        {
            var args = arg.Split(" ", 2);
            Id = new Id(args[1]);
        }
    }
}
