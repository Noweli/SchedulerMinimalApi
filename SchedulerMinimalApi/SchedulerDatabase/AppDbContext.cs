using Microsoft.EntityFrameworkCore;
using SchedulerDatabase.Models;

namespace SchedulerDatabase;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Schedule> Schedules { get; init; }
}