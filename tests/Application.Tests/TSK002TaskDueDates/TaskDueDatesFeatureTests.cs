using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.TSK002TaskDueDates;
using TodoApp.API.Controllers;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Builders;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.tests.Application.Tests.TSK002TaskDueDates
{
    [TestClass]
    public class TaskDueDatesFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(TaskDueDatesFeatureSetup)]);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(101).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Original Task")
                .WithId(1)
                .WithUserId(user.Id)
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
            }

            var updateRequest = new UpdateTaskRequest(
                "Updated Task Title",
                DateTime.UtcNow.AddDays(2),
                false
            );

            // Act
            var response = await TestClient.PutAsJsonAsync($"/api/tasks/{task.Id}", updateRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskItem>();
            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Task Title");
            result.DueDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(2), TimeSpan.FromMinutes(1));

            var dbContext = GetDbContext();
            var updatedTask = dbContext.Tasks.FirstOrDefault(x => x.Id == task.Id);
            updatedTask.Should().NotBeNull();
            updatedTask!.Title.Should().Be("Updated Task Title");
            updatedTask.DueDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(2), TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public async Task UpdateTask_ShouldCreateReminders_WhenDueDateSet()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(102).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task for Reminders")
                .WithId(2)
                .WithUserId(user.Id)
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
            }

            var dueDate = DateTime.UtcNow.AddDays(3);
            var updateRequest = new UpdateTaskRequest(null, dueDate, null);

            // Act
            var response = await TestClient.PutAsJsonAsync($"/api/tasks/{task.Id}", updateRequest);

            // Assert
            response.EnsureSuccessStatusCode();

            var dbContext = GetDbContext();
            var reminders = dbContext.TaskReminders.Where(r => r.TaskItemId == task.Id).ToList();
            reminders.Should().HaveCount(2);
            reminders.Should().Contain(r => r.Type == ReminderType.OneDayBefore);
            reminders.Should().Contain(r => r.Type == ReminderType.OneHourBefore);
        }

        [TestMethod]
        public async Task UpdateTask_ShouldMarkTaskCompleted_WhenIsCompletedSetToTrue()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(103).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task to Complete")
                .WithId(3)
                .WithUserId(user.Id)
                .DueInDays(1)
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
            }

            var updateRequest = new UpdateTaskRequest(null, null, true);

            // Act
            var response = await TestClient.PutAsJsonAsync($"/api/tasks/{task.Id}", updateRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskItem>();
            result.Should().NotBeNull();
            result!.IsCompleted.Should().BeTrue();
            result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            var dbContext = GetDbContext();
            var completedTask = dbContext.Tasks.FirstOrDefault(x => x.Id == task.Id);
            completedTask.Should().NotBeNull();
            completedTask!.IsCompleted.Should().BeTrue();
            completedTask.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public async Task GetTasksWithDueDates_ShouldReturnOverdueTasks_WhenOverdueOnlyTrue()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(104).Build();
            var overDueTask = TaskItemBuilder.Create()
                .WithTitle("Overdue Task")
                .WithId(4)
                .WithUserId(user.Id)
                .Overdue(1)
                .Build();

            var futureTask = TaskItemBuilder.Create()
                .WithTitle("Future Task")
                .WithId(5)
                .WithUserId(user.Id)
                .DueInDays(1)
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntities([overDueTask, futureTask]);
            }

            // Act
            var response = await TestClient.GetAsync("/api/tasks?overdueOnly=true");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskItem>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCountGreaterOrEqualTo(1);
                result.Should().Contain(t => t.Title == "Overdue Task");
                result.Should().NotContain(t => t.Title == "Future Task");
            }
        }

        [TestMethod]
        public async Task UpdateTask_ShouldReturnBadRequest_WhenTaskNotFound()
        {
            // Arrange
            var updateRequest = new UpdateTaskRequest("Non-existent Task", DateTime.UtcNow.AddDays(1), false);

            // Act
            var response = await TestClient.PutAsJsonAsync("/api/tasks/999", updateRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}