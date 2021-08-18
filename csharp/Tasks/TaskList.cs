using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public interface ICommandLine
    {
        string Command { get; }
        string[] Args { get; }
    }

    public sealed class TaskList
    {
        private readonly List<Project> projects;
        private readonly CommandFactory factory;
        private readonly IConsole console;

        public static void Main(string[] args)
        {
            new TaskList(new RealConsole()).Run();
        }

        public TaskList(IConsole console)
        {
            this.console = console;
            this.projects = new List<Project>();
            this.factory = new CommandFactory(projects, console);
        }

        public void Run()
        {
            ICommand command = null;
            while (command is not QuitCommand)
            {
                console.Write("> ");
                command = factory.Create(new CommandLine(console.ReadLine()));
                command.Execute();
            }
        }
	}
}
