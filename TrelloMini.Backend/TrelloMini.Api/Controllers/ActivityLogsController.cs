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
    public class ActivityLogsController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public ActivityLogsController(TrelloDbContext context)
        {
            _context = context;
        }

        // GET: api/ActivityLogs/board/{boardId}
        [HttpGet("board/{boardId}")]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetBoardActivityLogs(
            int boardId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] ActivityType? activityType = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user has access to this board
            var hasAccess = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId);

            if (!hasAccess)
            {
                var isOwner = await _context.Boards
                    .AnyAsync(b => b.Id == boardId && b.UserId == userId);

                if (!isOwner)
                {
                    return Forbid();
                }
            }

            var query = _context.ActivityLogs
                .Include(a => a.User)
                .Include(a => a.Board)
                .Include(a => a.List)
                .Include(a => a.Card)
                .Where(a => a.BoardId == boardId)
                .OrderByDescending(a => a.CreatedAt);

            // Apply filters
            if (activityType.HasValue)
            {
                query = (IOrderedQueryable<ActivityLog>)query.Where(a => a.ActivityType == activityType.Value);
            }

            if (startDate.HasValue)
            {
                query = (IOrderedQueryable<ActivityLog>)query.Where(a => a.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = (IOrderedQueryable<ActivityLog>)query.Where(a => a.CreatedAt <= endDate.Value);
            }

            var totalCount = await query.CountAsync();

            var activityLogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                activities = activityLogs,
                totalCount,
                page,
                pageSize,
                hasMore = (page * pageSize) < totalCount
            });
        }

        // GET: api/ActivityLogs/card/{cardId}
        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetCardActivityLogs(int cardId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Check if user has access to this card's board
            var card = await _context.Cards
                .Include(c => c.List)
                .ThenInclude(l => l.Board)
                .FirstOrDefaultAsync(c => c.Id == cardId);

            if (card == null)
            {
                return NotFound();
            }

            var boardId = card.List.BoardId;
            var hasAccess = await _context.BoardMembers
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId);

            if (!hasAccess)
            {
                var isOwner = await _context.Boards
                    .AnyAsync(b => b.Id == boardId && b.UserId == userId);

                if (!isOwner)
                {
                    return Forbid();
                }
            }

            var activityLogs = await _context.ActivityLogs
                .Include(a => a.User)
                .Where(a => a.CardId == cardId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(activityLogs);
        }

        // GET: api/ActivityLogs/user/me
        [HttpGet("user/me")]
        public async Task<ActionResult<IEnumerable<ActivityLog>>> GetMyActivityLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var query = _context.ActivityLogs
                .Include(a => a.Board)
                .Include(a => a.List)
                .Include(a => a.Card)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();

            var activityLogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                activities = activityLogs,
                totalCount,
                page,
                pageSize,
                hasMore = (page * pageSize) < totalCount
            });
        }

        // GET: api/ActivityLogs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityLog>> GetActivityLog(int id)
        {
            var activityLog = await _context.ActivityLogs
                .Include(a => a.User)
                .Include(a => a.Board)
                .Include(a => a.List)
                .Include(a => a.Card)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activityLog == null)
            {
                return NotFound();
            }

            return activityLog;
        }

        // POST: api/ActivityLogs
        [HttpPost]
        public async Task<ActionResult<ActivityLog>> CreateActivityLog(ActivityLog activityLog)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            activityLog.UserId = userId;
            activityLog.CreatedAt = DateTime.UtcNow;

            // Get IP address and user agent from request
            activityLog.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            activityLog.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActivityLog), new { id = activityLog.Id }, activityLog);
        }

        // DELETE: api/ActivityLogs/{id}
        [Authorize(Roles = "Admin")] // Only admins can delete activity logs
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivityLog(int id)
        {
            var activityLog = await _context.ActivityLogs.FindAsync(id);
            if (activityLog == null)
            {
                return NotFound();
            }

            _context.ActivityLogs.Remove(activityLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
