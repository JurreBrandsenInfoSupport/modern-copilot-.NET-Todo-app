using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class TaskReminderBuilder
    {
        private int _id = 1;
        private int _taskItemId = 1;
        private DateTime _scheduledDate = DateTime.UtcNow.AddDays(1);
        private bool _isSent = false;
        private DateTime? _sentAt = null;
        private ReminderType _type = ReminderType.OneDayBefore;

        public TaskReminderBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public TaskReminderBuilder WithTaskItemId(int taskItemId)
        {
            _taskItemId = taskItemId;
            return this;
        }

        public TaskReminderBuilder WithScheduledDate(DateTime scheduledDate)
        {
            _scheduledDate = scheduledDate;
            return this;
        }

        public TaskReminderBuilder ScheduledInDays(int days)
        {
            _scheduledDate = DateTime.UtcNow.AddDays(days);
            return this;
        }

        public TaskReminderBuilder ScheduledInHours(int hours)
        {
            _scheduledDate = DateTime.UtcNow.AddHours(hours);
            return this;
        }

        public TaskReminderBuilder OverdueBy(int hours)
        {
            _scheduledDate = DateTime.UtcNow.AddHours(-hours);
            return this;
        }

        public TaskReminderBuilder AsSent()
        {
            _isSent = true;
            _sentAt = DateTime.UtcNow;
            return this;
        }

        public TaskReminderBuilder AsPending()
        {
            _isSent = false;
            _sentAt = null;
            return this;
        }

        public TaskReminderBuilder WithType(ReminderType type)
        {
            _type = type;
            return this;
        }

        public TaskReminderBuilder AsOneDayBefore()
        {
            _type = ReminderType.OneDayBefore;
            return this;
        }

        public TaskReminderBuilder AsOneHourBefore()
        {
            _type = ReminderType.OneHourBefore;
            return this;
        }

        public TaskReminderBuilder AsOverdue()
        {
            _type = ReminderType.Overdue;
            return this;
        }

        public TaskReminder Build()
        {
            return new TaskReminder
            {
                Id = _id,
                TaskItemId = _taskItemId,
                ScheduledDate = _scheduledDate,
                IsSent = _isSent,
                SentAt = _sentAt,
                Type = _type
            };
        }

        public static TaskReminderBuilder Create() => new();
    }
}