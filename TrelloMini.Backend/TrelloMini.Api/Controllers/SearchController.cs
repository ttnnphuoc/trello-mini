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
    public class SearchController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public SearchController(TrelloDbContext context)
        {
            _context = context;
        }

        // GET: api/Search/global
        [HttpGet("global")]
        public async Task<ActionResult<object>> GlobalSearch(
            [FromQuery] string query,
            [FromQuery] string? searchType = null, // "all", "boards", "lists", "cards"
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Search query must be at least 2 characters long");
            }

            var searchQuery = query.ToLower();
            var results = new
            {
                boards = new List<Board>(),
                lists = new List<List>(),
                cards = new List<Card>(),
                totalResults = 0
            };

            // Get user's accessible board IDs
            var accessibleBoardIds = await _context.BoardMembers
                .Where(bm => bm.UserId == userId)
                .Select(bm => bm.BoardId)
                .Union(_context.Boards.Where(b => b.UserId == userId).Select(b => b.Id))
                .ToListAsync();

            // Search boards
            if (searchType == null || searchType == "all" || searchType == "boards")
            {
                var boardsQuery = _context.Boards
                    .Where(b => accessibleBoardIds.Contains(b.Id) &&
                           (b.Title.ToLower().Contains(searchQuery) ||
                            (b.Description != null && b.Description.ToLower().Contains(searchQuery))));

                results = new
                {
                    boards = await boardsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                    lists = results.lists,
                    cards = results.cards,
                    totalResults = results.totalResults + await boardsQuery.CountAsync()
                };
            }

            // Search lists
            if (searchType == null || searchType == "all" || searchType == "lists")
            {
                var listsQuery = _context.Lists
                    .Include(l => l.Board)
                    .Where(l => accessibleBoardIds.Contains(l.BoardId) &&
                           l.Title.ToLower().Contains(searchQuery));

                results = new
                {
                    boards = results.boards,
                    lists = await listsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                    cards = results.cards,
                    totalResults = results.totalResults + await listsQuery.CountAsync()
                };
            }

            // Search cards
            if (searchType == null || searchType == "all" || searchType == "cards")
            {
                var cardsQuery = _context.Cards
                    .Include(c => c.List)
                    .ThenInclude(l => l.Board)
                    .Where(c => accessibleBoardIds.Contains(c.List.BoardId) &&
                           (c.Title.ToLower().Contains(searchQuery) ||
                            (c.Description != null && c.Description.ToLower().Contains(searchQuery))));

                results = new
                {
                    boards = results.boards,
                    lists = results.lists,
                    cards = await cardsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(),
                    totalResults = results.totalResults + await cardsQuery.CountAsync()
                };
            }

            return Ok(new
            {
                query,
                results,
                page,
                pageSize
            });
        }

        // GET: api/Search/cards
        [HttpGet("cards")]
        public async Task<ActionResult<object>> SearchCards(
            [FromQuery] string? query,
            [FromQuery] int? boardId,
            [FromQuery] int? listId,
            [FromQuery] CardPriority? priority,
            [FromQuery] DateTime? dueDateStart,
            [FromQuery] DateTime? dueDateEnd,
            [FromQuery] bool? overdue,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Get user's accessible board IDs
            var accessibleBoardIds = await _context.BoardMembers
                .Where(bm => bm.UserId == userId)
                .Select(bm => bm.BoardId)
                .Union(_context.Boards.Where(b => b.UserId == userId).Select(b => b.Id))
                .ToListAsync();

            var cardsQuery = _context.Cards
                .Include(c => c.List)
                .ThenInclude(l => l.Board)
                .Where(c => accessibleBoardIds.Contains(c.List.BoardId));

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query))
            {
                var searchQuery = query.ToLower();
                cardsQuery = cardsQuery.Where(c =>
                    c.Title.ToLower().Contains(searchQuery) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchQuery)));
            }

            if (boardId.HasValue)
            {
                cardsQuery = cardsQuery.Where(c => c.List.BoardId == boardId.Value);
            }

            if (listId.HasValue)
            {
                cardsQuery = cardsQuery.Where(c => c.ListId == listId.Value);
            }

            if (priority.HasValue)
            {
                cardsQuery = cardsQuery.Where(c => c.Priority == priority.Value);
            }

            if (dueDateStart.HasValue)
            {
                cardsQuery = cardsQuery.Where(c => c.DueDate.HasValue && c.DueDate.Value >= dueDateStart.Value);
            }

            if (dueDateEnd.HasValue)
            {
                cardsQuery = cardsQuery.Where(c => c.DueDate.HasValue && c.DueDate.Value <= dueDateEnd.Value);
            }

            if (overdue.HasValue && overdue.Value)
            {
                var now = DateTime.UtcNow;
                cardsQuery = cardsQuery.Where(c => c.DueDate.HasValue && c.DueDate.Value < now);
            }

            var totalCount = await cardsQuery.CountAsync();
            var cards = await cardsQuery
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                cards,
                totalCount,
                page,
                pageSize,
                hasMore = (page * pageSize) < totalCount
            });
        }

        // GET: api/Search/boards
        [HttpGet("boards")]
        public async Task<ActionResult<object>> SearchBoards(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return BadRequest("Search query must be at least 2 characters long");
            }

            // Get user's accessible board IDs
            var accessibleBoardIds = await _context.BoardMembers
                .Where(bm => bm.UserId == userId)
                .Select(bm => bm.BoardId)
                .Union(_context.Boards.Where(b => b.UserId == userId).Select(b => b.Id))
                .ToListAsync();

            var searchQuery = query.ToLower();
            var boardsQuery = _context.Boards
                .Include(b => b.Lists)
                .Where(b => accessibleBoardIds.Contains(b.Id) &&
                       (b.Title.ToLower().Contains(searchQuery) ||
                        (b.Description != null && b.Description.ToLower().Contains(searchQuery))));

            var totalCount = await boardsQuery.CountAsync();
            var boards = await boardsQuery
                .OrderByDescending(b => b.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                boards,
                totalCount,
                page,
                pageSize,
                hasMore = (page * pageSize) < totalCount
            });
        }
    }
}
