using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Controllers
{
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
            return await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .OrderBy(b => b.CreatedAt)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetBoard(int id)
        {
            var board = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            return board;
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateBoard(Board board)
        {
            board.CreatedAt = DateTime.UtcNow;
            board.UpdatedAt = DateTime.UtcNow;
            
            _context.Boards.Add(board);
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

            board.UpdatedAt = DateTime.UtcNow;
            _context.Entry(board).State = EntityState.Modified;

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
            var board = await _context.Boards.FindAsync(id);
            if (board == null)
            {
                return NotFound();
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