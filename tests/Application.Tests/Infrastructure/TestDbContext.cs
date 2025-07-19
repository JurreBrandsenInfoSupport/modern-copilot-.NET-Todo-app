using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Tests.Infrastructure
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<User> Users => Set<User>();

        public async Task AddEntities<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().AddRange(entities);
            await SaveChangesAsync();
        }

        public async Task AddEntity<T>(T entity) where T : class
        {
            Set<T>().Add(entity);
            await SaveChangesAsync();
        }
    }
}
