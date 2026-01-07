using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public BoardsController(TrelloDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetBoards()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Get boards where user is owner or member
            var boards = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .Where(b => b.UserId == userId || 
                           b.Members.Any(m => m.UserId == userId && m.IsActive))
                .OrderBy(b => b.CreatedAt)
                .ToListAsync();

            return Ok(boards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetBoard(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var board = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .Include(b => b.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            // Check if user has access to this board
            var hasAccess = board.UserId == userId || 
                           board.Members.Any(m => m.UserId == userId && m.IsActive);

            if (!hasAccess)
            {
                return Forbid("You don't have access to this board");
            }

            return Ok(board);
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard(Board board)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            board.UserId = userId; // Set the creator as owner
            board.CreatedAt = DateTime.UtcNow;
            board.UpdatedAt = DateTime.UtcNow;
            
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            // Create owner membership
            var ownerMembership = new BoardMember
            {
                BoardId = board.Id,
                UserId = userId,
                Role = BoardRole.Owner,
                JoinedAt = DateTime.UtcNow,
                AcceptedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.BoardMembers.Add(ownerMembership);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, board);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(int id, Board board)
        {
            if (id != board.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user can edit this board
            var canEdit = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == id && bm.UserId == userId && 
                               bm.IsActive && bm.CanEditBoard) ||
                await _context.Boards.AnyAsync(b => b.Id == id && b.UserId == userId);

            if (!canEdit)
            {
                return Forbid("You don't have permission to edit this board");
            }

            var existingBoard = await _context.Boards.FindAsync(id);
            if (existingBoard == null)
            {
                return NotFound();
            }

            // Update only allowed properties
            existingBoard.Title = board.Title;
            existingBoard.Description = board.Description;
            existingBoard.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var board = await _context.Boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }

            // Only board owner can delete the board
            if (board.UserId != userId)
            {
                return Forbid("Only the board owner can delete this board");
            }

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BoardExists(int id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}