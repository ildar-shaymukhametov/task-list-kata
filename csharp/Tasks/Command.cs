using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public interface ICommand
    {
        void Execute();
    }

    public class ShowCommand : ICommand
    {
        private readonly IConsole console;
        private readonly List<Project> projects;

        public ShowCommand(IConsole console, List<Project> projects)
        {
            this.console = console;
            this.projects = projects;
        }

        public void Execute()
        {
            foreach (var project in projects)
            {
                console.WriteLine(project.Name);
                foreach (var task in project.Tasks)
                {
                    console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
                }
                console.WriteLine();
            }
        }
    }

    public class AddCommand : ICommand
    {
        private readonly ICommandLine commandLine;
        private readonly List<Project> projects;

        public AddCommand(ICommandLine commandLine, List<Project> projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var subcommand = commandLine.Args[0];
            var projectName = commandLine.Args[1];
            if (subcommand == "project")
            {
                projects.Add(new Project(projectName));
            }
            else if (subcommand == "task")
            {
                var project = projects.Find(x => x.Name == projectName);
                if (project == null)
                {
                    Console.WriteLine("Could not find a project with the name \"{0}\".", projectName);
                    return;
                }

                var description = string.Join(" ", commandLine.Args.Skip(2));
                project.Tasks.Add(new Task { Id = Id.GetNextId(), Description = description, Done = false });
            }
        }
    }

    public class CheckCommand : ICommand
    {
        private readonly ICommandLine commandLine;
        private readonly List<Project> projects;
        private readonly IConsole console;

        public CheckCommand(ICommandLine commandLine, List<Project> projects, IConsole console)
        {
            this.commandLine = commandLine;
            this.projects = projects;
            this.console = console;
        }

        public void Execute()
        {
            SetDone(new Id(commandLine.Args[0]), true);
        }

        private void SetDone(Id id, bool done)
        {
            var identifiedTask = projects
                .SelectMany(project => project.Tasks)
                .FirstOrDefault(task => task.Id == id.Value);
            if (identifiedTask == null)
            {
                console.WriteLine("Could not find a task with an ID of {0}.", id);
                return;
            }

            identifiedTask.Done = done;
        }
    }

    public class UncheckCommand : ICommand
    {
        private readonly ICommandLine commandLine;
        private readonly List<Project> projects;
        private readonly IConsole console;

        public UncheckCommand(ICommandLine commandLine, List<Project> projects, IConsole console)
        {
            this.commandLine = commandLine;
            this.projects = projects;
            this.console = console;
        }

        public void Execute()
        {
            SetDone(new Id(commandLine.Args[0]), false);
        }

        private void SetDone(Id id, bool done)
        {
            var identifiedTask = projects
                .SelectMany(project => project.Tasks)
                .FirstOrDefault(task => task.Id == id.Value);
            if (identifiedTask == null)
            {
                console.WriteLine("Could not find a task with an ID of {0}.", id);
                return;
            }

            identifiedTask.Done = done;
        }
    }

    public class HelpCommand : ICommand
    {
        private readonly IConsole console;

        public HelpCommand(IConsole console)
        {
            this.console = console;
        }

        public void Execute()
        {
            console.WriteLine("Commands:");
            console.WriteLine("  show");
            console.WriteLine("  add project <project name>");
            console.WriteLine("  add task <project name> <task description>");
            console.WriteLine("  check <task ID>");
            console.WriteLine("  uncheck <task ID>");
            console.WriteLine();
        }
    }

    public class ErrorCommand : ICommand
    {
        private readonly ICommandLine commandLine;
        private readonly IConsole console;

        public ErrorCommand(ICommandLine commandLine, IConsole console)
        {
            this.commandLine = commandLine;
            this.console = console;
        }

        public void Execute()
        {
            console.WriteLine("I don't know what the command \"{0}\" is.", commandLine.Command);
        }
    }

    public class QuitCommand : ICommand
    {
        public void Execute()
        {
        }
    }

    public class CommandFactory
    {
        private readonly List<Project> projects;
        private readonly IConsole console;

        public CommandFactory(List<Project> projects, IConsole console)
        {
            this.projects = projects;
            this.console = console;
        }

        public ICommand Create(ICommandLine commandLine)
        {
            ICommand result = null;
            switch (commandLine.Command)
            {
                case "show":
                    result = new ShowCommand(console, projects);
                    break;
                case "add":
                    result = new AddCommand(commandLine, projects);
                    break;
                case "check":
                    result = new CheckCommand(commandLine, projects, console);
                    break;
                case "uncheck":
                    result = new UncheckCommand(commandLine, projects, console);
                    break;
                case "help":
                    result = new HelpCommand(console);
                    break;
                case "quit":
                    result = new QuitCommand();
                    break;
                default:
                    result = new ErrorCommand(commandLine, console);
                    break;
            }

            return result;
        }
    }
}
