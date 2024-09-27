using System;
using System.Collections.Generic;

namespace Tasks
{
	public class Task
	{
		public string Id { get; set; }

		public string Description { get; set; }

		public bool Done { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
