using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public enum ActivityType
    {
        // Board activities
        BoardCreated,
        BoardUpdated,
        BoardDeleted,
        BoardMemberAdded,
        BoardMemberRemoved,
        BoardMemberRoleChanged,

        // List activities
        ListCreated,
        ListUpdated,
        ListDeleted,
        ListMoved,

        // Card activities
        CardCreated,
        CardUpdated,
        CardDeleted,
        CardMoved,
        CardPriorityChanged,
        CardDueDateChanged,
        CardAssigned,
        CardUnassigned,

        // Collaboration activities
        CommentAdded,
        CommentUpdated,
        CommentDeleted,
        AttachmentAdded,
        AttachmentDeleted,

        // User activities
        UserJoined,
        UserLeft
    }

    public class ActivityLog
    {
        public int Id { get; set; }

        [Required]
        public ActivityType ActivityType { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // JSON data for additional details
        [StringLength(5000)]
        public string? Data { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // User who performed the action
        public int? UserId { get; set; }
        public User? User { get; set; }

        // Related entities
        public int? BoardId { get; set; }
        public Board? Board { get; set; }

        public int? ListId { get; set; }
        public List? List { get; set; }

        public int? CardId { get; set; }
        public Card? Card { get; set; }

        // IP address for security audit
        [StringLength(45)]
        public string? IpAddress { get; set; }

        // User agent for tracking client
        [StringLength(500)]
        public string? UserAgent { get; set; }
    }
}
