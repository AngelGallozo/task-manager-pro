namespace TaskManager.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Api.DTOs;



[ApiController]
[Route("api/boards/{boardId}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TasksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Obtener todas las tareas de un board
    [HttpGet]
    public IActionResult GetTasks(Guid boardId)
    {
        var userId = GetUserId();

        var boardExists = _context.Boards
            .Any(b => b.Id == boardId && b.UserId == userId);

        if (!boardExists)
            return NotFound("Board not found");

        var tasks = _context.Tasks
            .Where(t => t.BoardId == boardId)
            .ToList();

        return Ok(tasks);
    }

    // Obtener tarea específica
    [HttpGet("{taskId}")]
    public IActionResult GetTask(Guid boardId, Guid taskId)
    {
        var userId = GetUserId();

        var task = _context.Tasks
            .Where(t => t.Id == taskId && t.BoardId == boardId)
            .Join(_context.Boards,
                  t => t.BoardId,
                  b => b.Id,
                  (t, b) => new { Task = t, Board = b })
            .FirstOrDefault(x => x.Board.UserId == userId);

        if (task == null)
            return NotFound();

        return Ok(task.Task);
    }

    // Crear tarea
    [HttpPost]
    public IActionResult CreateTask(Guid boardId, [FromBody] CreateTaskRequest request)
    {
        var userId = GetUserId();

        var board = _context.Boards
            .FirstOrDefault(b => b.Id == boardId && b.UserId == userId);

        if (board == null)
            return NotFound("Board not found");

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Status = TaskStatus.Pending,
            BoardId = boardId
        };

        _context.Tasks.Add(task);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetTask),
            new { boardId = boardId, taskId = task.Id },
            task);
    }

    // Actualizar tarea
    [HttpPut("{taskId}")]
    public IActionResult UpdateTask(Guid boardId, Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        var userId = GetUserId();

        var task = _context.Tasks
            .Where(t => t.Id == taskId && t.BoardId == boardId)
            .Join(_context.Boards,
                  t => t.BoardId,
                  b => b.Id,
                  (t, b) => new { Task = t, Board = b })
            .FirstOrDefault(x => x.Board.UserId == userId);

        if (task == null)
            return NotFound();

        task.Task.Title = request.Title;
        task.Task.Description = request.Description;
        task.Task.Status = request.Status;

        _context.SaveChanges();

        return NoContent();
    }

    // Eliminar tarea
    [HttpDelete("{taskId}")]
    public IActionResult DeleteTask(Guid boardId, Guid taskId)
    {
        var userId = GetUserId();

        var task = _context.Tasks
            .Where(t => t.Id == taskId && t.BoardId == boardId)
            .Join(_context.Boards,
                  t => t.BoardId,
                  b => b.Id,
                  (t, b) => new { Task = t, Board = b })
            .FirstOrDefault(x => x.Board.UserId == userId);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task.Task);
        _context.SaveChanges();

        return NoContent();
    }

    //Helper para obtener UserId desde JWT
    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            throw new UnauthorizedAccessException("UserId claim not found");

        return Guid.Parse(claim.Value);
    }
}