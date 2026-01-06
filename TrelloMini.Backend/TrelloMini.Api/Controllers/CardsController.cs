using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardsController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public CardsController(TrelloDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Card>>> GetCards()
        {
            return await _context.Cards
                .OrderBy(c => c.Position)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Card>> GetCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return card;
        }

        [HttpGet("list/{listId}")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCardsByList(int listId)
        {
            return await _context.Cards
                .Where(c => c.ListId == listId)
                .OrderBy(c => c.Position)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Card>> CreateCard(Card card)
        {
            card.CreatedAt = DateTime.UtcNow;
            card.UpdatedAt = DateTime.UtcNow;
            
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, Card card)
        {
            if (id != card.Id)
            {
                return BadRequest();
            }

            card.UpdatedAt = DateTime.UtcNow;
            _context.Entry(card).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveCard(int id, [FromBody] MoveCardRequest request)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            card.ListId = request.ListId;
            card.Position = request.Position;
            card.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.Id == id);
        }
    }

    public class MoveCardRequest
    {
        public int ListId { get; set; }
        public int Position { get; set; }
    }
}