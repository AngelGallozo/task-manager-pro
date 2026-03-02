using TaskManager.Domain.Entities;

namespace TaskManager.Api.DTOs;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}