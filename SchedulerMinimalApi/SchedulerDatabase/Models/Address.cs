using System.ComponentModel.DataAnnotations;

namespace SchedulerDatabase.Models;

public record Address
{
    public int Id { get; init; }
    [Required] [MaxLength(50)] public string? City { get; init; }
    [MaxLength(50)] public string? Street { get; init; }
    [MaxLength(10)] public string? HomeNo { get; init; }

    public Schedule Schedule { get; init; } = null!;
}