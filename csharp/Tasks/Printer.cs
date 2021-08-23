using System.Collections.Generic;

namespace Tasks
{
    public interface IPrinter
    {
        void Print(Dictionary<string, Task[]> data);
    }

    public class Printer : IPrinter
    {
        private readonly IConsole console;

        public Printer(IConsole console)
        {
            this.console = console;
        }

        public void Print(Dictionary<string, Task[]> data)
        {
            foreach (var group in data)
            {
                console.WriteLine(group.Key);
                foreach (var task in group.Value)
                {
                    console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
                }
                console.WriteLine();
            }
        }
    }
}
