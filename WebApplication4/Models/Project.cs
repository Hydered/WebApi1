namespace WebApplication4.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public bool IsArchived { get; set; }
    }
}