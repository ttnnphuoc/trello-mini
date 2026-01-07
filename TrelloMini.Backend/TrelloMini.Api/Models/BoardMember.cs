using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public enum BoardRole
    {
        Owner = 0,
        Admin = 1,
        Member = 2,
        Viewer = 3
    }

    public class BoardMember
    {
        public int Id { get; set; }
        
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        public BoardRole Role { get; set; } = BoardRole.Member;
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        // Invitation tracking
        public int? InvitedByUserId { get; set; }
        public User? InvitedBy { get; set; }
        public DateTime? InvitedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        
        // Permissions based on role
        public bool CanEditBoard => Role <= BoardRole.Admin;
        public bool CanManageMembers => Role <= BoardRole.Admin;
        public bool CanCreateLists => Role <= BoardRole.Member;
        public bool CanEditCards => Role <= BoardRole.Member;
        public bool CanDeleteBoard => Role == BoardRole.Owner;
    }
}