using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class TaskItemBuilder
    {
        private int _id = 1;
        private string _title = "Test Task";
        private bool _isCompleted = false;

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
            return this;
        }

        public TaskItemBuilder AsCompleted()
        {
            _isCompleted = true;
            return this;
        }

        public TaskItemBuilder AsIncomplete()
        {
            _isCompleted = false;
            return this;
        }

        public TaskItem Build()
        {
            return new TaskItem
            {
                Id = _id,
                Title = _title,
                IsCompleted = _isCompleted
            };
        }

        public static TaskItemBuilder Create() => new();
    }
}
