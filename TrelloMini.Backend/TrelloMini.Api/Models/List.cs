using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public class List
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        public int Position { get; set; }
        
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}