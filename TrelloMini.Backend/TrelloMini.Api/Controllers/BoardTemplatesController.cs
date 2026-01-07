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
    public class BoardTemplatesController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public BoardTemplatesController(TrelloDbContext context)
        {
            _context = context;
        }

        // GET: api/BoardTemplates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardTemplate>>> GetTemplates(
            [FromQuery] string? category = null,
            [FromQuery] bool includePrivate = false)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var query = _context.BoardTemplates
                .Include(t => t.Lists)
                .ThenInclude(l => l.Cards)
                .Include(t => t.CreatedBy)
                .AsQueryable();

            if (includePrivate)
            {
                // Show public templates and user's private templates
                query = query.Where(t => t.IsPublic || t.CreatedByUserId == userId);
            }
            else
            {
                // Show only public templates
                query = query.Where(t => t.IsPublic);
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category == category);
            }

            var templates = await query
                .OrderByDescending(t => t.UsageCount)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(templates);
        }

        // GET: api/BoardTemplates/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.BoardTemplates
                .Where(t => t.IsPublic)
                .Select(t => t.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/BoardTemplates/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardTemplate>> GetTemplate(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var template = await _context.BoardTemplates
                .Include(t => t.Lists)
                .ThenInclude(l => l.Cards)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound();
            }

            // Check if user has access to this template
            if (!template.IsPublic && template.CreatedByUserId != userId)
            {
                return Forbid();
            }

            return template;
        }

        // POST: api/BoardTemplates
        [HttpPost]
        public async Task<ActionResult<BoardTemplate>> CreateTemplate(BoardTemplate template)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            template.CreatedByUserId = userId;
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;
            template.UsageCount = 0;

            _context.BoardTemplates.Add(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }

        // POST: api/BoardTemplates/from-board/{boardId}
        [HttpPost("from-board/{boardId}")]
        public async Task<ActionResult<BoardTemplate>> CreateTemplateFromBoard(
            int boardId,
            [FromBody] CreateTemplateRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var board = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .FirstOrDefaultAsync(b => b.Id == boardId);

            if (board == null)
            {
                return NotFound("Board not found");
            }

            // Check if user owns the board or is a member
            var hasAccess = board.UserId == userId ||
                await _context.BoardMembers.AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId);

            if (!hasAccess)
            {
                return Forbid();
            }

            // Create template from board
            var template = new BoardTemplate
            {
                Name = request.TemplateName ?? board.Title,
                Description = request.TemplateDescription ?? board.Description,
                Category = request.Category ?? "General",
                IsPublic = request.IsPublic,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UsageCount = 0
            };

            // Copy lists
            foreach (var list in board.Lists.OrderBy(l => l.Position))
            {
                var templateList = new TemplateList
                {
                    Title = list.Title,
                    Position = list.Position,
                    Template = template
                };

                // Copy cards
                foreach (var card in list.Cards.OrderBy(c => c.Position))
                {
                    templateList.Cards.Add(new TemplateCard
                    {
                        Title = card.Title,
                        Description = card.Description,
                        Position = card.Position,
                        Priority = card.Priority
                    });
                }

                template.Lists.Add(templateList);
            }

            _context.BoardTemplates.Add(template);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }

        // POST: api/BoardTemplates/{id}/apply
        [HttpPost("{id}/apply")]
        public async Task<ActionResult<Board>> ApplyTemplate(int id, [FromBody] ApplyTemplateRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var template = await _context.BoardTemplates
                .Include(t => t.Lists)
                .ThenInclude(l => l.Cards)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                return NotFound("Template not found");
            }

            // Check if user has access to this template
            if (!template.IsPublic && template.CreatedByUserId != userId)
            {
                return Forbid();
            }

            // Create new board from template
            var board = new Board
            {
                Title = request.BoardTitle ?? template.Name,
                Description = request.BoardDescription ?? template.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Copy lists from template
            foreach (var templateList in template.Lists.OrderBy(l => l.Position))
            {
                var list = new List
                {
                    Title = templateList.Title,
                    Position = templateList.Position,
                    Board = board,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Copy cards from template
                foreach (var templateCard in templateList.Cards.OrderBy(c => c.Position))
                {
                    list.Cards.Add(new Card
                    {
                        Title = templateCard.Title,
                        Description = templateCard.Description,
                        Position = templateCard.Position,
                        Priority = templateCard.Priority,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                board.Lists.Add(list);
            }

            _context.Boards.Add(board);

            // Increment usage count
            template.UsageCount++;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoard", "Boards", new { id = board.Id }, board);
        }

        // PUT: api/BoardTemplates/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, BoardTemplate template)
        {
            if (id != template.Id)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var existingTemplate = await _context.BoardTemplates.FindAsync(id);
            if (existingTemplate == null)
            {
                return NotFound();
            }

            // Only creator can update template
            if (existingTemplate.CreatedByUserId != userId)
            {
                return Forbid();
            }

            existingTemplate.Name = template.Name;
            existingTemplate.Description = template.Description;
            existingTemplate.Category = template.Category;
            existingTemplate.IsPublic = template.IsPublic;
            existingTemplate.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/BoardTemplates/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var template = await _context.BoardTemplates.FindAsync(id);
            if (template == null)
            {
                return NotFound();
            }

            // Only creator can delete template
            if (template.CreatedByUserId != userId)
            {
                return Forbid();
            }

            _context.BoardTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateTemplateRequest
    {
        public string? TemplateName { get; set; }
        public string? TemplateDescription { get; set; }
        public string? Category { get; set; }
        public bool IsPublic { get; set; } = false;
    }

    public class ApplyTemplateRequest
    {
        public string? BoardTitle { get; set; }
        public string? BoardDescription { get; set; }
    }
}
