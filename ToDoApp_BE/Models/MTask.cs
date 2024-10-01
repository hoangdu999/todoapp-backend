namespace ToDoApp_BE.Models
{
    public class MTask
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; } = "unfinish";
    }
}
