using System.Collections.Generic;

namespace Tasks
{
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
}
