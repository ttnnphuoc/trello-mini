using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public enum InvitationStatus
    {
        Pending = 0,
        Accepted = 1,
        Declined = 2,
        Expired = 3,
        Cancelled = 4
    }

    public class BoardInvitation
    {
        public int Id { get; set; }
        
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        public int? InvitedUserId { get; set; }  // If user already exists
        public User? InvitedUser { get; set; }
        
        public int InvitedByUserId { get; set; }
        public User InvitedBy { get; set; } = null!;
        
        [Required]
        public BoardRole ProposedRole { get; set; } = BoardRole.Member;
        
        [Required]
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        
        [Required]
        [StringLength(64)]
        public string Token { get; set; } = string.Empty;  // Unique invitation token
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);  // 7 days expiry
        public DateTime? RespondedAt { get; set; }
        
        [StringLength(500)]
        public string? Message { get; set; }  // Optional invitation message
        
        public bool IsValid => Status == InvitationStatus.Pending && 
                              ExpiresAt > DateTime.UtcNow;
    }
}