using System;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public interface ICommand
    {
        void Execute();
    }

    public class ViewByProjectCommand : ICommand
    {
        private readonly IConsole console;
        private readonly Projects projects;
        private readonly IPrinter printer;

        public ViewByProjectCommand(IConsole console, Projects projects, IPrinter printer)
        {
            this.console = console;
            this.projects = projects;
            this.printer = printer;
        }

        public void Execute()
        {
            var data = projects.ToDictionary(x => x.Name, x => x.Tasks.ToArray());
            printer.Print(data);
        }
    }

    public class ViewByDeadlineCommand : ICommand
    {
        private readonly IConsole console;
        private readonly Projects projects;
        private readonly IPrinter printer;

        public ViewByDeadlineCommand(IConsole console, Projects projects, IPrinter printer)
        {
            this.console = console;
            this.projects = projects;
            this.printer = printer;
        }

        public void Execute()
        {
            var data = projects.GetAllTasks()
                .Where(x => x.Deadline != null)
                .GroupBy(x => x.Deadline)
                .ToDictionary(x => x.Key?.ToString("dd.MM.yyyy"), x => x.ToArray());
            printer.Print(data);
        }
    }

    public class AddProjectCommand : ICommand
    {
        private readonly AddProjectCommandLine commandLine;
        private readonly Projects projects;

        public AddProjectCommand(AddProjectCommandLine commandLine, Projects projects)
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
        private readonly Projects projects;

        public AddTaskCommand(AddTaskCommandLine commandLine, Projects projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var project = projects.Find(commandLine.Project);
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
        private readonly IdCommandLine commandLine;
        private readonly Projects projects;
        private readonly IConsole console;

        public CheckCommand(IdCommandLine commandLine, Projects projects, IConsole console)
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
            var identifiedTask = projects.GetTaskById(id);
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
        private readonly IdCommandLine commandLine;
        private readonly Projects projects;
        private readonly IConsole console;

        public UncheckCommand(IdCommandLine commandLine, Projects projects, IConsole console)
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
            var identifiedTask = projects.GetTaskById(id);
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
        private readonly Projects projects;

        public DeadlineCommand(DeadlineCommandLine commandLine, Projects projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var task = projects.GetTaskById(commandLine.Id);
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
        private readonly Projects projects;
        private readonly DateTime today;

        public TodayCommand(IConsole console, Projects projects, DateTime today)
        {
            this.console = console;
            this.projects = projects;
            this.today = today;
        }

        public void Execute()
        {
            projects.GetAllTasks()
                .Where(x => x.Deadline == today)
                .ToList()
                .ForEach(x => console.WriteLine(x.Description));
            console.WriteLine();
        }
    }

    public class IdCommand : ICommand
    {
        private readonly IdsCommandLine commandLine;
        private readonly Projects projects;

        public IdCommand(IdsCommandLine commandLine, Projects projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            var task = projects.GetTaskById(commandLine.OldId);
            task.Id = commandLine.NewId;
        }
    }

    public class DeleteCommand : ICommand
    {
        private readonly IdCommandLine commandLine;
        private readonly Projects projects;

        public DeleteCommand(IdCommandLine commandLine, Projects projects)
        {
            this.commandLine = commandLine;
            this.projects = projects;
        }

        public void Execute()
        {
            projects.DeleteTaskById(commandLine.Id);
        }
    }

    public class CommandFactory
    {
        private readonly Projects projects;
        private readonly IConsole console;
        private readonly DateTime today;

        public CommandFactory(Projects projects, IConsole console, DateTime today)
        {
            this.projects = projects;
            this.console = console;
            this.today = today;
        }

        public ICommand Create(string arg)
        {
            ICommand result = null;
            if (arg.StartsWith("view by project"))
            {
                result = new ViewByProjectCommand(console, projects, new Printer(console));
            }
            else if (arg.StartsWith("view by deadline"))
            {
                result = new ViewByDeadlineCommand(console, projects, new Printer(console));
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
                result = new CheckCommand(new IdCommandLine(arg), projects, console);
            }
            else if (arg.StartsWith("uncheck"))
            {
                result = new UncheckCommand(new IdCommandLine(arg), projects, console);
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
            else if (arg.StartsWith("id"))
            {
                result = new IdCommand(new IdsCommandLine(arg), projects);
            }
            else if (arg.StartsWith("delete"))
            {
                result = new DeleteCommand(new IdCommandLine(arg), projects);
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
