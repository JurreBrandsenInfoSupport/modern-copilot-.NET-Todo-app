using System;

namespace TodoApp.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
