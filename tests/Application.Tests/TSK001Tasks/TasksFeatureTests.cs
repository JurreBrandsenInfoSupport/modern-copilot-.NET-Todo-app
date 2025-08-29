using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.TSK001Tasks;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Builders;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.tests.Application.Tests.TSK001Tasks
{
    [TestClass]
    public class TasksFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(TasksFeatureSetup)]);
        }

        [TestMethod]
        public async Task CreateTask_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("taskuser").WithId(101).Build();
            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
            }
            var command = new CreateTaskCommand("Test Task Title", user.Id);

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/tasks", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskItem>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Title.Should().Be("Test Task Title");
                result.IsCompleted.Should().BeFalse();
            }

            // Verify in database
            var dbContext = GetDbContext();
            var createdTask = dbContext.Tasks.FirstOrDefault(x => x.Title == "Test Task Title");
            createdTask.Should().NotBeNull();
            createdTask!.Title.Should().Be("Test Task Title");
            createdTask.IsCompleted.Should().BeTrue();
        }

        [TestMethod]
        public async Task CreateTask_ShouldCreateTaskInDatabase_WhenValidRequest()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("dbuser").WithId(102).Build();
            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
            }
            var command = new CreateTaskCommand("Database Test Task", user.Id);

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/tasks", command);

            // Assert
            response.EnsureSuccessStatusCode();

            // Separate context for assertion
            var dbContext = GetDbContext();
            var task = dbContext.Tasks.FirstOrDefault(x => x.Title == "Database Test Task");
            task.Should().NotBeNull();
            task!.Title.Should().Be("Database Test Task");
            task.IsCompleted.Should().BeFalse();
            task.Id.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task GetAllTasks_ShouldReturnOk_WhenTasksExist()
        {
            // Arrange
            var tasks = new[]
            {
                TaskItemBuilder.Create().WithTitle("Task 1").WithId(1).Build(),
                TaskItemBuilder.Create().WithTitle("Task 2").WithId(2).AsCompleted().Build(),
                TaskItemBuilder.Create().WithTitle("Task 3").WithId(3).Build()
            };

            using var context = GetTestDbContext();
            await context.AddEntities(tasks);

            // Act
            var response = await TestClient.GetAsync("/api/tasks");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCountGreaterOrEqualTo(3);

                var task1 = result.FirstOrDefault(t => t.Title == "Task 1");
                task1.Should().NotBeNull();
                if (task1 != null)
                    task1.IsCompleted.Should().BeFalse();

                var task2 = result.FirstOrDefault(t => t.Title == "Task 2");
                task2.Should().NotBeNull();
                if (task2 != null)
                    task2.IsCompleted.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task GetAllTasks_ShouldReturnEmptyList_WhenNoTasksExist()
        {
            // Act
            var response = await TestClient.GetAsync("/api/tasks");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            result!.Should().NotBeNull();
            result.Should().BeOfType<List<TaskItem>>();
        }
        [TestMethod]
        public async Task GetTasksByUser_ShouldReturnOnlyUserTasks()
        {
            var user1 = UserBuilder.Create().WithUsername("userA").WithId(201).Build();
            var user2 = UserBuilder.Create().WithUsername("userB").WithId(202).Build();
            var tasks = new[]
            {
                TaskItemBuilder.Create().WithTitle("UserA Task 1").WithId(11).WithUserId(user1.Id).Build(),
                TaskItemBuilder.Create().WithTitle("UserA Task 2").WithId(12).WithUserId(user1.Id).Build(),
                TaskItemBuilder.Create().WithTitle("UserB Task 1").WithId(21).WithUserId(user2.Id).Build()
            };
            using var context = GetTestDbContext();
            await context.AddEntity(user1);
            await context.AddEntity(user2);
            await context.AddEntities(tasks);

            var response = await TestClient.GetAsync($"/api/tasks?userId={user1.Id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCount(2);
                result.All(t => t.UserId == user1.Id).Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task CreateTask_ShouldReturnBadRequest_WhenUserDoesNotExist()
        {
            var command = new CreateTaskCommand("Task with invalid user", 9999);
            var response = await TestClient.PostAsJsonAsync("/api/tasks", command);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GetTasksByUser_ShouldReturnEmptyList_WhenUserHasNoTasks()
        {
            var user = UserBuilder.Create().WithUsername("emptyuser").WithId(301).Build();
            using var context = GetTestDbContext();
            await context.AddEntity(user);

            var response = await TestClient.GetAsync($"/api/tasks?userId={user.Id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
