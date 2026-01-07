using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrelloMini.Api.Data;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Services
{
    public interface IActivityLogService
    {
        Task LogActivityAsync(
            ActivityType activityType,
            string description,
            int? userId = null,
            int? boardId = null,
            int? listId = null,
            int? cardId = null,
            object? data = null,
            string? ipAddress = null,
            string? userAgent = null);

        Task LogBoardActivityAsync(int boardId, ActivityType activityType, string description, int? userId = null, object? data = null);
        Task LogCardActivityAsync(int cardId, ActivityType activityType, string description, int? userId = null, object? data = null);
        Task LogListActivityAsync(int listId, ActivityType activityType, string description, int? userId = null, object? data = null);
    }

    public class ActivityLogService : IActivityLogService
    {
        private readonly TrelloDbContext _context;

        public ActivityLogService(TrelloDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(
            ActivityType activityType,
            string description,
            int? userId = null,
            int? boardId = null,
            int? listId = null,
            int? cardId = null,
            object? data = null,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var activityLog = new ActivityLog
            {
                ActivityType = activityType,
                Description = description,
                UserId = userId,
                BoardId = boardId,
                ListId = listId,
                CardId = cardId,
                Data = data != null ? JsonSerializer.Serialize(data) : null,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(activityLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogBoardActivityAsync(int boardId, ActivityType activityType, string description, int? userId = null, object? data = null)
        {
            await LogActivityAsync(activityType, description, userId, boardId, data: data);
        }

        public async Task LogCardActivityAsync(int cardId, ActivityType activityType, string description, int? userId = null, object? data = null)
        {
            // Get the card's list and board IDs
            var card = await _context.Cards
                .Include(c => c.List)
                .FirstOrDefaultAsync(c => c.Id == cardId);

            if (card != null)
            {
                await LogActivityAsync(
                    activityType,
                    description,
                    userId,
                    card.List.BoardId,
                    card.ListId,
                    cardId,
                    data: data);
            }
        }

        public async Task LogListActivityAsync(int listId, ActivityType activityType, string description, int? userId = null, object? data = null)
        {
            // Get the list's board ID
            var list = await _context.Lists
                .FirstOrDefaultAsync(l => l.Id == listId);

            if (list != null)
            {
                await LogActivityAsync(
                    activityType,
                    description,
                    userId,
                    list.BoardId,
                    listId,
                    data: data);
            }
        }
    }
}
