using TodoApp.Domain.Entities;

namespace TodoApp.tests.Application.Tests.Builders
{
    public class UserBuilder
    {
        private int _id = 1;
        private string _username = "testuser";

        public UserBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public UserBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public User Build()
        {
            return new User
            {
                Id = _id,
                Username = _username
            };
        }

        public static UserBuilder Create() => new();
    }
}
