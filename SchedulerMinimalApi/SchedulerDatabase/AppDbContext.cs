using Microsoft.EntityFrameworkCore;
using SchedulerDatabase.Models;

namespace SchedulerDatabase;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<Schedule> Schedules { get; init; }
    public DbSet<Address> Addresses { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Schedule>()
            .HasOne(schedule => schedule.Address)
            .WithOne(address => address.Schedule)
            .HasForeignKey<Address>(address => address.ScheduleId)
            .IsRequired();
    }
}