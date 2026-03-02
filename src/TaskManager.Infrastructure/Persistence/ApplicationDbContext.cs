using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
}