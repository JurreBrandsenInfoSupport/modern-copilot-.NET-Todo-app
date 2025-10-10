using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class TaskItemBuilder
    {
        private int _id = 1;
        private string _title = "Test Task";
        private bool _isCompleted = false;
        // Default value 0 for _userId is acceptable for tests (see PR discussion)
        private int _userId = 0;
        private DateTime? _dueDate = null;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime? _completedAt = null;

        public TaskItemBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public TaskItemBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public TaskItemBuilder WithCompletedStatus(bool isCompleted)
        {
            _isCompleted = isCompleted;
            if (isCompleted && !_completedAt.HasValue)
            {
                _completedAt = DateTime.UtcNow;
            }
            else if (!isCompleted)
            {
                _completedAt = null;
            }
            return this;
        }

        public TaskItemBuilder AsCompleted()
        {
            _isCompleted = true;
            _completedAt = DateTime.UtcNow;
            return this;
        }

        public TaskItemBuilder AsIncomplete()
        {
            _isCompleted = false;
            _completedAt = null;
            return this;
        }

        public TaskItemBuilder WithUserId(int userId)
        {
            _userId = userId;
            return this;
        }

        public TaskItemBuilder WithDueDate(DateTime? dueDate)
        {
            _dueDate = dueDate;
            return this;
        }

        public TaskItemBuilder DueInDays(int days)
        {
            _dueDate = DateTime.UtcNow.AddDays(days);
            return this;
        }

        public TaskItemBuilder DueInHours(int hours)
        {
            _dueDate = DateTime.UtcNow.AddHours(hours);
            return this;
        }

        public TaskItemBuilder Overdue(int daysAgo)
        {
            _dueDate = DateTime.UtcNow.AddDays(-daysAgo);
            return this;
        }

        public TaskItemBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public TaskItemBuilder WithCompletedAt(DateTime? completedAt)
        {
            _completedAt = completedAt;
            return this;
        }

        public TaskItem Build()
        {
            return new TaskItem
            {
                Id = _id,
                Title = _title,
                IsCompleted = _isCompleted,
                UserId = _userId,
                DueDate = _dueDate,
                CreatedAt = _createdAt,
                CompletedAt = _completedAt
            };
        }

        public static TaskItemBuilder Create() => new();
    }
}
