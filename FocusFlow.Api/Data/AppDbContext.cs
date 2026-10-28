using FocusFlow.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FocusFlow.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(task =>
        {
            task.Property(t => t.Title).HasMaxLength(100).IsRequired();
            task.Property(t => t.Description).HasMaxLength(500);
            task.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            task.HasIndex(t => new { t.Status, t.DueDate });
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Username).HasMaxLength(50).IsRequired();
            user.HasIndex(u => u.Username).IsUnique();
        });
    }
}
