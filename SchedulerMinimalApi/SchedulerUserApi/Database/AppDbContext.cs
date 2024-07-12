using Microsoft.EntityFrameworkCore;
using SchedulerUserApi.Database.Models;

namespace SchedulerUserApi.Database;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
}