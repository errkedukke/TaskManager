using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Persistence.DatabaseContext;

public class TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
    : DbContext(options)
{
    public DbSet<TaskItem> TaskItems { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
