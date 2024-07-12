using System.ComponentModel.DataAnnotations;

namespace SchedulerUserApi.Database.Models;

public record User
{
    public int Id { get; init; }
    [Required] [MaxLength(50)] public string? Username { get; init; }
}