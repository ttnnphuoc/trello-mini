using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public enum NotificationType
    {
        BoardInvitation = 0,
        BoardMemberAdded = 1,
        CardAssigned = 2,
        CardDueSoon = 3,
        CardOverdue = 4,
        CardUpdated = 5,
        CardMoved = 6,
        ListCreated = 7,
        BoardShared = 8,
        MentionInCard = 9
    }

    public class Notification
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Message { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReadAt { get; set; }
        
        // Related entity references
        public int? BoardId { get; set; }
        public Board? Board { get; set; }
        
        public int? CardId { get; set; }
        public Card? Card { get; set; }
        
        public int? ListId { get; set; }
        public List? List { get; set; }
        
        // Actor who triggered the notification
        public int? ActorUserId { get; set; }
        public User? ActorUser { get; set; }
        
        // Additional data (JSON)
        [StringLength(2000)]
        public string? Data { get; set; }
        
        // Expiry for temporary notifications
        public DateTime? ExpiresAt { get; set; }
        
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
    }
}