using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public enum CardPriority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public class Card
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        public int Position { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public CardPriority Priority { get; set; } = CardPriority.None;
        
        public int ListId { get; set; }
        public List? List { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}