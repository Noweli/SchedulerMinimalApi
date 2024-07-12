using Microsoft.EntityFrameworkCore;
using SchedulerMinimalApi.Database.Models;

namespace SchedulerMinimalApi.Database;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
}