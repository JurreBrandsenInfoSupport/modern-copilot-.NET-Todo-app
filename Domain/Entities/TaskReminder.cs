namespace TodoApp.Domain.Entities
{
    public class TaskReminder
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
        public DateTime ScheduledDate { get; set; }
        public bool IsSent { get; set; } = false;
        public DateTime? SentAt { get; set; }
        public ReminderType Type { get; set; }
    }

    public enum ReminderType
    {
        OneDayBefore = 1,
        OneHourBefore = 2,
        Overdue = 3
    }
}