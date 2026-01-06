using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public class Card
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public int Position { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public int ListId { get; set; }
        public List List { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}