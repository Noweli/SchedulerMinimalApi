namespace SchedulerDatabase.Models.DTOs;

public record ScheduleDto
{
    public int UserId { get; init; }
    public string? ScheduleName { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}