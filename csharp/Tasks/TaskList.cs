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

    public class Project
    {
        public List<Task> Tasks { get; }
        public string Name { get; }
	
        public Project(string name)
        {
            Name = name;
            Tasks = new List<Task>();
        }
    }

    public sealed class TaskList
	{
		private const string QUIT = "quit";

        private readonly List<Project> projects;
		private readonly IConsole console;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this.console = console;
            this.projects = new List<Project>();
		}

		public void Run()
		{
			while (true) {
				console.Write("> ");
				var command = console.ReadLine();
				if (command == QUIT) {
					break;
				}
                Execute(new CommandLine(command));
			}
		}

        private void Execute(ICommandLine commandLine)
		{
            switch (commandLine.Command)
            {
			case "show":
				Show();
				break;
			case "add":
                    Add(commandLine);
				break;
			case "check":
                    Check(commandLine);
				break;
			case "uncheck":
                    Uncheck(commandLine);
				break;
			case "help":
				Help();
				break;
			default:
                    Error(commandLine);
				break;
			}
		}

		private void Show()
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

        private void Add(ICommandLine commandLine)
		{
            var subcommand = commandLine.Args[0];
			if (subcommand == "project") {
                AddProject(commandLine.Args[1]);
			} else if (subcommand == "task") {
                var projectName = commandLine.Args[1];
                var project = projects.Find(x => x.Name == projectName);
                if (project == null)
                {
                    Console.WriteLine("Could not find a project with the name \"{0}\".", projectName);
                    return;
                }

                var description = string.Join(" ", commandLine.Args.Skip(2));
                project.Tasks.Add(new Task { Id = NextId(), Description = description, Done = false });
			}
		}

		private void AddProject(string name)
		{
            projects.Add(new Project(name));
		}

        private void Check(ICommandLine commandLine)
		{
            SetDone(commandLine.Args[0], true);
		}

        private void Uncheck(ICommandLine commandLine)
		{
            SetDone(commandLine.Args[0], false);
		}

		private void SetDone(string idString, bool done)
		{
			int id = int.Parse(idString);
            var identifiedTask = projects
                .SelectMany(project => project.Tasks)
                .FirstOrDefault(task => task.Id == id);
			if (identifiedTask == null) {
				console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void Help()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  add project <project name>");
			console.WriteLine("  add task <project name> <task description>");
			console.WriteLine("  check <task ID>");
			console.WriteLine("  uncheck <task ID>");
			console.WriteLine();
		}

        private void Error(ICommandLine commandLine)
		{
            console.WriteLine("I don't know what the command \"{0}\" is.", commandLine.Command);
		}

		private long NextId()
		{
			return ++lastId;
		}
	}
}
