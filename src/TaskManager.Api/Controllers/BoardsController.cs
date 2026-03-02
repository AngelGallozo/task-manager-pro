using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Domain.Entities;
using TaskManager.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] //Todo el controller requiere token
public class BoardsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BoardsController(ApplicationDbContext context)
    {
        _context = context;
    }

    //Obtener tableros del usuario logueado
    [HttpGet]
    public IActionResult GetMyBoards()
    {
        var userId = GetUserId();

        var boards = _context.Boards
            .Where(b => b.UserId == userId)
            .ToList();

        return Ok(boards);
    }

    //Crear nuevo tablero
    [HttpPost]
    public IActionResult CreateBoard([FromBody] CreateBoardRequest request)
    {
        var userId = GetUserId();

        var board = new Board
        {
            Name = request.Name,
            UserId = userId
        };

        _context.Boards.Add(board);
        _context.SaveChanges();

        return CreatedAtAction(nameof(GetMyBoards), new { id = board.Id }, board);
    }

    //Helper para obtener UserId del token
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("UserId claim not found");

        return Guid.Parse(userIdClaim.Value);
    }

    [HttpGet("{id}")]
    public IActionResult GetBoardById(Guid id)
    {
        var userId = GetUserId();

        var board = _context.Boards
            .Where(b => b.Id == id && b.UserId == userId)
            .FirstOrDefault();

        if (board == null)
            return NotFound();

        return Ok(board);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateBoard(Guid id, [FromBody] UpdateBoardRequest request)
    {
        var userId = GetUserId();

        var board = _context.Boards
            .FirstOrDefault(b => b.Id == id && b.UserId == userId);

        if (board == null)
            return NotFound();

        board.Name = request.Name;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteBoard(Guid id)
    {
        var userId = GetUserId();

        var board = _context.Boards
            .FirstOrDefault(b => b.Id == id && b.UserId == userId);

        if (board == null)
            return NotFound();

        _context.Boards.Remove(board);
        _context.SaveChanges();

        return NoContent();
    }
}