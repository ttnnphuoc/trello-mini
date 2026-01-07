using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TrelloMini.Api.Hubs
{
    [Authorize]
    public class BoardHub : Hub
    {
        public async Task JoinBoardGroup(string boardId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Board_{boardId}");
            
            // Notify others that a user joined
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            await Clients.Group($"Board_{boardId}")
                .SendAsync("UserJoined", new { UserId = userId, Username = username, ConnectionId = Context.ConnectionId });
        }

        public async Task LeaveBoardGroup(string boardId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Board_{boardId}");
            
            // Notify others that a user left
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;
            
            await Clients.Group($"Board_{boardId}")
                .SendAsync("UserLeft", new { UserId = userId, Username = username, ConnectionId = Context.ConnectionId });
        }

        public async Task NotifyCardMoved(string boardId, object cardMoveData)
        {
            // Broadcast card movement to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("CardMoved", cardMoveData);
        }

        public async Task NotifyCardUpdated(string boardId, object cardData)
        {
            // Broadcast card updates to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("CardUpdated", cardData);
        }

        public async Task NotifyCardCreated(string boardId, object cardData)
        {
            // Broadcast new card creation to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("CardCreated", cardData);
        }

        public async Task NotifyCardDeleted(string boardId, object cardData)
        {
            // Broadcast card deletion to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("CardDeleted", cardData);
        }

        public async Task NotifyListCreated(string boardId, object listData)
        {
            // Broadcast new list creation to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("ListCreated", listData);
        }

        public async Task NotifyListDeleted(string boardId, object listData)
        {
            // Broadcast list deletion to all users in the board group except the sender
            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("ListDeleted", listData);
        }

        public async Task SendTypingIndicator(string boardId, string cardId, bool isTyping)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            await Clients.GroupExcept($"Board_{boardId}", Context.ConnectionId)
                .SendAsync("UserTyping", new { 
                    UserId = userId, 
                    Username = username, 
                    CardId = cardId, 
                    IsTyping = isTyping 
                });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Handle user disconnection - could notify groups they were part of
            await base.OnDisconnectedAsync(exception);
        }
    }
}