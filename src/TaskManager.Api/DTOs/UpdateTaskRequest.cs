namespace TaskManager.Api.DTOs;
using TaskManager.Domain.Entities;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
}