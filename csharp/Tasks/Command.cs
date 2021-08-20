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

    public class AddProjectCommand : ICommand
    {
        private readonly AddProjectCommandLine commandLine;
        private readonly List<Project> projects;

        public AddProjectCommand(AddProjectCommandLine commandLine, List<Project> projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            projects.Add(new Project(commandLine.Project));
        }
    }

    public class AddTaskCommand : ICommand
    {
        private readonly AddTaskCommandLine commandLine;
        private readonly List<Project> projects;

        public AddTaskCommand(AddTaskCommandLine commandLine, List<Project> projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var project = projects.Find(x => x.Name == commandLine.Project);
            if (project == null)
            {
                Console.WriteLine("Could not find a project with the name \"{0}\".", commandLine.Project);
                return;
            }

            project.Tasks.Add(new Task { Id = Id.GetNextId(), Description = commandLine.Description, Done = false });
        }
    }

    public class CheckCommand : ICommand
    {
        private readonly CheckCommandLine commandLine;
        private readonly List<Project> projects;
        private readonly IConsole console;

        public CheckCommand(CheckCommandLine commandLine, List<Project> projects, IConsole console)
        {
            this.commandLine = commandLine;
            this.projects = projects;
            this.console = console;
        }

        public void Execute()
        {
            SetDone(commandLine.Id, true);
        }

        private void SetDone(Id id, bool done)
        {
            var identifiedTask = projects
                .SelectMany(project => project.Tasks)
                .FirstOrDefault(task => task.Id == id);
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
        private readonly CheckCommandLine commandLine;
        private readonly List<Project> projects;
        private readonly IConsole console;

        public UncheckCommand(CheckCommandLine commandLine, List<Project> projects, IConsole console)
        {
            this.commandLine = commandLine;
            this.projects = projects;
            this.console = console;
        }

        public void Execute()
        {
            SetDone(commandLine.Id, false);
        }

        private void SetDone(Id id, bool done)
        {
            var identifiedTask = projects
                .SelectMany(project => project.Tasks)
                .FirstOrDefault(task => task.Id == id);
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
            console.WriteLine("I don't know what the command \"{0}\" is.", commandLine.Arg);
        }
    }

    public class QuitCommand : ICommand
    {
        public void Execute()
        {
        }
    }

    public class DeadlineCommand : ICommand
    {
        private readonly DeadlineCommandLine commandLine;
        private readonly List<Project> projects;

        public DeadlineCommand(DeadlineCommandLine commandLine, List<Project> projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var task = projects.SelectMany(x => x.Tasks).FirstOrDefault(x => x.Id == commandLine.Id);
            if (task == null)
            {
                Console.WriteLine("Could not find a task with the id \"{0}\".", commandLine.Id);
                return;
            }

            task.Deadline = commandLine.Deadline;
        }
    }

    public class TodayCommand : ICommand
    {
        private readonly IConsole console;
        private readonly List<Project> projects;
        private readonly DateTime today;

        public TodayCommand(IConsole console, List<Project> projects, DateTime today)
        {
            this.console = console;
            this.projects = projects;
            this.today = today;
        }

        public void Execute()
        {
            projects
                .SelectMany(x => x.Tasks)
                .Where(x => x.Deadline == today)
                .ToList()
                .ForEach(x => console.WriteLine(x.Description));
            console.WriteLine();
        }
    }

    public class CommandFactory
    {
        private readonly List<Project> projects;
        private readonly IConsole console;
        private readonly DateTime today;

        public CommandFactory(List<Project> projects, IConsole console, DateTime today)
        {
            this.projects = projects;
            this.console = console;
            this.today = today;
        }

        public ICommand Create(string arg)
        {
            ICommand result = null;
            if (arg.StartsWith("show"))
            {
                result = new ShowCommand(console, projects);
            }
            else if (arg.StartsWith("add task"))
            {
                result = new AddTaskCommand(new AddTaskCommandLine(arg), projects);
            }
            else if (arg.StartsWith("add project"))
            {
                result = new AddProjectCommand(new AddProjectCommandLine(arg), projects);
            }
            else if (arg.StartsWith("check"))
            {
                result = new CheckCommand(new CheckCommandLine(arg), projects, console);
            }
            else if (arg.StartsWith("uncheck"))
            {
                result = new UncheckCommand(new CheckCommandLine(arg), projects, console);
            }
            else if (arg.StartsWith("help"))
            {
                result = new HelpCommand(console);
            }
            else if (arg.StartsWith("deadline"))
            {
                result = new DeadlineCommand(new DeadlineCommandLine(arg), projects);
            }
            else if (arg.StartsWith("today"))
            {
                result = new TodayCommand(console, projects, today);
            }
            else if (arg.StartsWith("quit"))
            {
                result = new QuitCommand();
            }
            else
            {
                result = new ErrorCommand(new CommandLine(arg), console);
            }

            return result;
        }
    }
}
