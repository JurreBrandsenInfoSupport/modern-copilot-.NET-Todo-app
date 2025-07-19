using System;
using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class CommentBuilder
    {
        private int _id = 1;
        private int _taskItemId = 1;
        private TaskItem _taskItem = null!;
        private int _userId = 1;
        private User _user = null!;
        private string _text = "Test comment";
        private DateTime _createdAt = DateTime.UtcNow;

        public CommentBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public CommentBuilder WithTaskItemId(int taskItemId)
        {
            _taskItemId = taskItemId;
            return this;
        }

        public CommentBuilder WithTaskItem(TaskItem taskItem)
        {
            _taskItem = taskItem;
            _taskItemId = taskItem.Id;
            return this;
        }

        public CommentBuilder WithUserId(int userId)
        {
            _userId = userId;
            return this;
        }

        public CommentBuilder WithUser(User user)
        {
            _user = user;
            _userId = user.Id;
            return this;
        }

        public CommentBuilder WithText(string text)
        {
            _text = text;
            return this;
        }

        public CommentBuilder WithCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public Comment Build()
        {
            return new Comment
            {
                Id = _id,
                TaskItemId = _taskItemId,
                TaskItem = _taskItem,
                UserId = _userId,
                User = _user,
                Text = _text,
                CreatedAt = _createdAt
            };
        }

        public static CommentBuilder Create() => new();
    }
}
