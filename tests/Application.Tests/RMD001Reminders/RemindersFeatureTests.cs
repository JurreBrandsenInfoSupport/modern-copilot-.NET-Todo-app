using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.RMD001Reminders;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Builders;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.tests.Application.Tests.RMD001Reminders
{
    [TestClass]
    public class RemindersFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(RemindersFeatureSetup)]);
        }

        [TestMethod]
        public async Task CreateReminder_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(201).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task with Reminder")
                .WithId(10)
                .WithUserId(user.Id)
                .DueInDays(2)
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
            }

            var command = new CreateReminderCommand(
                task.Id,
                DateTime.UtcNow.AddDays(1),
                ReminderType.OneDayBefore
            );

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/reminders", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskReminder>();
            result.Should().NotBeNull();
            result!.TaskItemId.Should().Be(task.Id);
            result.Type.Should().Be(ReminderType.OneDayBefore);

            var dbContext = GetDbContext();
            var reminder = dbContext.TaskReminders.FirstOrDefault(r => r.TaskItemId == task.Id);
            reminder.Should().NotBeNull();
            reminder!.Type.Should().Be(ReminderType.OneDayBefore);
        }

        [TestMethod]
        public async Task GetPendingReminders_ShouldReturnPendingReminders_WhenRemindersExist()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(202).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task with Pending Reminder")
                .WithId(11)
                .WithUserId(user.Id)
                .Build();

            var pendingReminder = TaskReminderBuilder.Create()
                .WithId(1)
                .WithTaskItemId(task.Id)
                .OverdueBy(1) // 1 hour overdue
                .AsPending()
                .AsOneDayBefore()
                .Build();

            var sentReminder = TaskReminderBuilder.Create()
                .WithId(2)
                .WithTaskItemId(task.Id)
                .OverdueBy(2)
                .AsSent()
                .AsOneHourBefore()
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
                await context.AddEntities([pendingReminder, sentReminder]);
            }

            // Act
            var response = await TestClient.GetAsync("/api/reminders/pending");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskReminder>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCountGreaterOrEqualTo(1);
                result.Should().Contain(r => r.Id == pendingReminder.Id);
                result.Should().NotContain(r => r.Id == sentReminder.Id);
            }
        }

        [TestMethod]
        public async Task GetRemindersByTask_ShouldReturnTaskReminders_WhenTaskExists()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(203).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task with Multiple Reminders")
                .WithId(12)
                .WithUserId(user.Id)
                .Build();

            var reminder1 = TaskReminderBuilder.Create()
                .WithId(3)
                .WithTaskItemId(task.Id)
                .AsOneDayBefore()
                .Build();

            var reminder2 = TaskReminderBuilder.Create()
                .WithId(4)
                .WithTaskItemId(task.Id)
                .AsOneHourBefore()
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
                await context.AddEntities([reminder1, reminder2]);
            }

            // Act
            var response = await TestClient.GetAsync($"/api/reminders/task/{task.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TaskReminder>>();
            result.Should().NotBeNull();
            if (result != null)
            {
                result.Should().HaveCount(2);
                result.Should().Contain(r => r.Type == ReminderType.OneDayBefore);
                result.Should().Contain(r => r.Type == ReminderType.OneHourBefore);
            }
        }

        [TestMethod]
        public async Task MarkReminderSent_ShouldUpdateReminderStatus_WhenValidRequest()
        {
            // Arrange
            var user = UserBuilder.Create().WithUsername("testuser").WithId(204).Build();
            var task = TaskItemBuilder.Create()
                .WithTitle("Task for Marking Reminder")
                .WithId(13)
                .WithUserId(user.Id)
                .Build();

            var reminder = TaskReminderBuilder.Create()
                .WithId(5)
                .WithTaskItemId(task.Id)
                .AsPending()
                .AsOneDayBefore()
                .Build();

            using (var context = GetTestDbContext())
            {
                await context.AddEntity(user);
                await context.AddEntity(task);
                await context.AddEntity(reminder);
            }

            // Act
            var response = await TestClient.PutAsync($"/api/reminders/{reminder.Id}/mark-sent", null);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TaskReminder>();
            result.Should().NotBeNull();
            result!.IsSent.Should().BeTrue();
            result.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            var dbContext = GetDbContext();
            var updatedReminder = dbContext.TaskReminders.FirstOrDefault(r => r.Id == reminder.Id);
            updatedReminder.Should().NotBeNull();
            updatedReminder!.IsSent.Should().BeTrue();
            updatedReminder.SentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [TestMethod]
        public async Task CreateReminder_ShouldReturnBadRequest_WhenTaskNotFound()
        {
            // Arrange
            var command = new CreateReminderCommand(
                999, // Non-existent task ID
                DateTime.UtcNow.AddDays(1),
                ReminderType.OneDayBefore
            );

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/reminders", command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task MarkReminderSent_ShouldReturnBadRequest_WhenReminderNotFound()
        {
            // Act
            var response = await TestClient.PutAsync("/api/reminders/999/mark-sent", null);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}