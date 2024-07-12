namespace SchedulerDatabase.Models.DTOs;

public record AddressDto
{
    public string? City { get; init; }
    public string? Street { get; init; }
    public string? HomeNo { get; init; }
    public int ScheduleId { get; init; }
}