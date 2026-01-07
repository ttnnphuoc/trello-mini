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
    public class NotificationsController : ControllerBase
    {
        private readonly TrelloDbContext _context;

        public NotificationsController(TrelloDbContext context)
        {
            _context = context;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20,
            [FromQuery] bool? unreadOnly = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var query = _context.Notifications
                .Include(n => n.Board)
                .Include(n => n.Card)
                .Include(n => n.List)
                .Include(n => n.ActorUser)
                .Where(n => n.UserId == userId && !n.IsExpired)
                .OrderByDescending(n => n.CreatedAt);

            if (unreadOnly == true)
            {
                query = (IOrderedQueryable<Notification>)query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            return Ok(new 
            { 
                notifications, 
                totalCount, 
                page, 
                pageSize,
                hasMore = (page * pageSize) < totalCount
            });
        }

        // GET: api/Notifications/unread-count
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var count = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsExpired);

            return Ok(new { unreadCount = count });
        }

        // PUT: api/Notifications/{id}/read
        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // PUT: api/Notifications/mark-all-read
        [HttpPut("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsExpired)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { markedCount = unreadNotifications.Count });
        }

        // DELETE: api/Notifications/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Notifications/create (Internal use for system notifications)
        [HttpPost("create")]
        public async Task<ActionResult> CreateNotification(CreateNotificationRequest request)
        {
            // This endpoint would typically be called internally or by admin users
            // For now, we'll allow any authenticated user to create notifications
            var notification = new Notification
            {
                UserId = request.UserId,
                Type = request.Type,
                Title = request.Title,
                Message = request.Message,
                BoardId = request.BoardId,
                CardId = request.CardId,
                ListId = request.ListId,
                ActorUserId = request.ActorUserId,
                Data = request.Data,
                ExpiresAt = request.ExpiresAt,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(notification);
        }
    }

    public class CreateNotificationRequest
    {
        public int UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public int? BoardId { get; set; }
        public int? CardId { get; set; }
        public int? ListId { get; set; }
        public int? ActorUserId { get; set; }
        public string? Data { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}