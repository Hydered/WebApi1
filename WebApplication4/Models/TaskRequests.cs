using System;

namespace WebApplication4.Models
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }

        public string Owner { get; set; }
    }

    public class CreateTaskRequest
    {
        public int ProjectId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskRequest
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        public string Status { get; set; }
    }
}