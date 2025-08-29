namespace TodoApp.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}