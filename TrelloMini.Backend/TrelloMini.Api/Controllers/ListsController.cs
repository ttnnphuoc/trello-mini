using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListsController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public ListsController(TrelloDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> GetLists()
        {
            return await _context.Lists
                .Include(l => l.Cards)
                .OrderBy(l => l.Position)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List>> GetList(int id)
        {
            var list = await _context.Lists
                .Include(l => l.Cards)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        [HttpGet("board/{boardId}")]
        public async Task<ActionResult<IEnumerable<List>>> GetListsByBoard(int boardId)
        {
            return await _context.Lists
                .Where(l => l.BoardId == boardId)
                .Include(l => l.Cards)
                .OrderBy(l => l.Position)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<List>> CreateList(List list)
        {
            list.CreatedAt = DateTime.UtcNow;
            list.UpdatedAt = DateTime.UtcNow;
            
            _context.Lists.Add(list);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetList), new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateList(int id, List list)
        {
            if (id != list.Id)
            {
                return BadRequest();
            }

            list.UpdatedAt = DateTime.UtcNow;
            _context.Entry(list).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(int id)
        {
            var list = await _context.Lists.FindAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListExists(int id)
        {
            return _context.Lists.Any(e => e.Id == id);
        }
    }
}