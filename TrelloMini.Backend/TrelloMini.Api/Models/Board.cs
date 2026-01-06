using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public class Board
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // User relationship
        public int? UserId { get; set; }
        public User? User { get; set; }
        
        public ICollection<List> Lists { get; set; } = new List<List>();
    }
}