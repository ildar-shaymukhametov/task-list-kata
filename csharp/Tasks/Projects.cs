using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tasks
{
    public class Projects : IEnumerable<Project>
    {
        private readonly List<Project> _projects;
        private readonly DateTime today;

        public Projects(DateTime today)
        {
            _projects = new List<Project>();
            this.today = today;
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
            return _projects.SelectMany(project => project.Tasks).FirstOrDefault(task => task.Id == id);
        }

        public List<Task> GetTasksDueToday()
        {
            return _projects
                .SelectMany(x => x.Tasks)
                .Where(x => x.Deadline == today)
                .ToList();
        }

        public IEnumerator<Project> GetEnumerator()
        {
            foreach (var item in _projects)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
