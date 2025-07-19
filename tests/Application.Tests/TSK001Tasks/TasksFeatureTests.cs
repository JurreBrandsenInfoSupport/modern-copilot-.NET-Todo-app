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
            var command = new CreateTaskCommand("Test Task Title");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/tasks", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskItem>();
            result!.Should().NotBeNull();
            result.Title.Should().Be("Test Task Title");
            result.IsCompleted.Should().BeFalse();

            // Verify in database
            var dbContext = GetDbContext();
            var createdTask = dbContext.Tasks.FirstOrDefault(x => x.Title == "Test Task Title");
            createdTask.Should().NotBeNull();
            createdTask!.Title.Should().Be("Test Task Title");
            createdTask.IsCompleted.Should().BeFalse();
        }

        [TestMethod]
        public async Task CreateTask_ShouldCreateTaskInDatabase_WhenValidRequest()
        {
            // Arrange
            var command = new CreateTaskCommand("Database Test Task");

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
            result!.Should().NotBeNull();
            result.Should().HaveCountGreaterOrEqualTo(3);

            var task1 = result.FirstOrDefault(t => t.Title == "Task 1");
            task1.Should().NotBeNull();
            task1!.IsCompleted.Should().BeFalse();

            var task2 = result.FirstOrDefault(t => t.Title == "Task 2");
            task2.Should().NotBeNull();
            task2!.IsCompleted.Should().BeTrue();
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
    }
}
