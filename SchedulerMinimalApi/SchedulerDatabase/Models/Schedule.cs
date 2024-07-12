using System.ComponentModel.DataAnnotations;

namespace SchedulerDatabase.Models;

public record Schedule
{
    public int Id { get; init; }
    [Required] public int UserId { get; init; }
    [Required] [MaxLength(50)] public string? ScheduleName { get; init; }
    [Required] public DateTime Start { get; init; }
    [Required] public DateTime End { get; init; }

    public Address? Address { get; init; }
}