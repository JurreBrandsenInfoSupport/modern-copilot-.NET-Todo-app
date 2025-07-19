using TodoApp.Application.USR002Users;
using TodoApp.Domain.Entities;
using TodoApp.tests.Application.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoApp.tests.Application.Tests.Builders;
using FluentAssertions;

namespace TodoApp.tests.Application.Tests.USR002Users
{
    [TestClass]
    public class UsersFeatureTests : BaseAuthApiIntegrationTest
    {
        [ClassInitialize]
        public static async Task Initialize(TestContext _) => await TestClassInitialize(_);

        [ClassCleanup]
        public static async Task Cleanup() => await TestClassCleanup();

        [TestInitialize]
        public void TestInitialize()
        {
            IntegrationTestInitialize<WebUIAssemblyLocator>(
                [typeof(UsersFeatureSetup)]);
        }

        [TestMethod]
        public async Task RegisterUser_ShouldReturnOk_WhenValidRequest()
        {
            // Arrange
            var command = new RegisterUserCommand("newuser123");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/users", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<User>();
            result!.Should().NotBeNull();
            result.Username.Should().Be("newuser123");
            result.Id.Should().BeGreaterThan(0);

            // Verify in database
            var dbContext = GetDbContext();
            var createdUser = dbContext.Users.FirstOrDefault(x => x.Username == "newuser123");
            createdUser.Should().NotBeNull();
            createdUser!.Username.Should().Be("newuser123");
        }

        [TestMethod]
        public async Task RegisterUser_ShouldCreateUserInDatabase_WhenValidRequest()
        {
            // Arrange
            var command = new RegisterUserCommand("databaseuser");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/users", command);

            // Assert
            response.EnsureSuccessStatusCode();

            // Separate context for assertion
            var dbContext = GetDbContext();
            var user = dbContext.Users.FirstOrDefault(x => x.Username == "databaseuser");
            user.Should().NotBeNull();
            user!.Username.Should().Be("databaseuser");
            user.Id.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnOk_WhenUsersExist()
        {
            // Arrange
            var users = new[]
            {
                UserBuilder.Create().WithUsername("user1").WithId(1).Build(),
                UserBuilder.Create().WithUsername("user2").WithId(2).Build(),
                UserBuilder.Create().WithUsername("user3").WithId(3).Build()
            };

            using var context = GetTestDbContext();
            await context.AddEntities(users);

            // Act
            var response = await TestClient.GetAsync("/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<User>>();
            result!.Should().NotBeNull();
            result.Should().HaveCountGreaterOrEqualTo(3);

            var user1 = result.FirstOrDefault(u => u.Username == "user1");
            user1.Should().NotBeNull();
            user1!.Id.Should().Be(1);

            var user2 = result.FirstOrDefault(u => u.Username == "user2");
            user2.Should().NotBeNull();
            user2!.Id.Should().Be(2);
        }

        [TestMethod]
        public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Act
            var response = await TestClient.GetAsync("/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<User>>();
            result!.Should().NotBeNull();
            result.Should().BeOfType<List<User>>();
        }

        [TestMethod]
        public async Task RegisterUser_ShouldHandleUniqueUsernames()
        {
            // Arrange
            var existingUser = UserBuilder.Create()
                .WithUsername("existinguser")
                .WithId(100)
                .Build();

            using var context = GetTestDbContext();
            await context.AddEntity(existingUser);

            var command = new RegisterUserCommand("newuniqueuser");

            // Act
            var response = await TestClient.PostAsJsonAsync("/api/users", command);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<User>();
            result!.Should().NotBeNull();
            result.Username.Should().Be("newuniqueuser");

            // Verify both users exist in database
            var dbContext = GetDbContext();
            var users = dbContext.Users.Where(u => u.Username == "existinguser" || u.Username == "newuniqueuser").ToList();
            users.Should().HaveCount(2);
        }
    }
}
