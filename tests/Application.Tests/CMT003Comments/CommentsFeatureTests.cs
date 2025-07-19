using TodoApp.tests.Application.Tests.Builders;

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.Application.CMT003Comments;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Infrastructure;

namespace TodoApp.Application.Tests.CMT003Comments
{
    [TestClass]
    public class CommentsFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(TodoApp.Application.CMT003Comments.CommentsFeatureSetup)]);
        }

        [TestMethod]
        public async Task AddComment_ShouldReturnOk_WhenValidRequest()
        {
            var dbContext = GetDbContext();
            var user = UserBuilder.Create().WithUsername("TestUser").Build();
            var task = TaskItemBuilder.Create().WithTitle("Test Task").Build();
            dbContext.Users.Add(user);
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            var command = new AddCommentCommand(task.Id, user.Id, "Test comment");
            var response = await TestClient.PostAsJsonAsync("/api/comments", command);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Comment>();
            result!.Text.Should().Be("Test comment");
            result.TaskItemId.Should().Be(task.Id);
            result.UserId.Should().Be(user.Id);
        }

        [TestMethod]
        public async Task GetComments_ShouldReturnCommentsForTask()
        {
            var dbContext = GetDbContext();
            var user = UserBuilder.Create().WithUsername("TestUser").Build();
            var task = TaskItemBuilder.Create().WithTitle("Test Task").Build();
            dbContext.Users.Add(user);
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            var comment = CommentBuilder.Create()
                .WithTaskItemId(task.Id)
                .WithUserId(user.Id)
                .WithText("Comment 1")
                .WithCreatedAt(DateTime.UtcNow)
                .Build();
            dbContext.Comments.Add(comment);
            dbContext.SaveChanges();

            var response = await TestClient.GetAsync($"/api/comments?taskItemId={task.Id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<Comment>>();
            result.Should().NotBeNull();
            result.Should().ContainSingle(c => c.Text == "Comment 1");
        }

        [TestMethod]
        public async Task AddComment_EmptyText_ShouldReturnBadRequest()
        {
            var dbContext = GetDbContext();
            var user = UserBuilder.Create().WithUsername("TestUser").Build();
            var task = TaskItemBuilder.Create().WithTitle("Test Task").Build();
            dbContext.Users.Add(user);
            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();

            var command = new AddCommentCommand(task.Id, user.Id, "");
            var response = await TestClient.PostAsJsonAsync("/api/comments", command);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddComment_InvalidUserOrTask_ShouldReturnBadRequest()
        {
            var command = new AddCommentCommand(999, 999, "Test");
            var response = await TestClient.PostAsJsonAsync("/api/comments", command);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
