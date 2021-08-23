using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public sealed class TaskList
    {
        private readonly Projects projects;
        private readonly CommandFactory factory;
        private readonly IConsole console;

        public static void Main(string[] args)
        {
            new TaskList(new RealConsole(), DateTime.Now).Run();
        }

        public TaskList(IConsole console, DateTime today)
        {
            this.console = console;
            this.projects = new Projects();
            this.factory = new CommandFactory(projects, console, today);
        }

        public void Run()
        {
            ICommand command = null;
            while (command is not QuitCommand)
            {
                console.Write("> ");
                command = factory.Create(console.ReadLine());
                command.Execute();
            }
        }
	}
}
