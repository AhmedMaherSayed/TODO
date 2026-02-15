using Microsoft.EntityFrameworkCore;
using TODO.Models;

namespace TODO.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.Property(e => e.Title)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasMaxLength(240);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
