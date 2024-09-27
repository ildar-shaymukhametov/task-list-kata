using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public class Projects
    {
        private readonly List<Project> _projects;

        public Projects()
        {
            _projects = new List<Project>();
        }

        public void Add(Project project)
        {
            _projects.Add(project);
        }

        public Project Find(string name)
        {
            return _projects.Find(x => x.Name == name);
        }

        public Task GetTaskById(Id id)
        {
            return GetAllTasks().FirstOrDefault(task => task.Id == id);
        }

        public Dictionary<string, Task[]> ToDictionary()
        {
            return _projects.ToDictionary(x => x.Name, x => x.Tasks.ToArray());
        }

        public List<Task> GetAllTasks()
        {
            return _projects.SelectMany(x => x.Tasks).ToList();
        }

        public void DeleteTaskById(Id id)
        {
            var task = GetTaskById(id);
            _projects.ForEach(p => p.Tasks.Remove(task));
        }
    }
}
