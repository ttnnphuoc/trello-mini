using System.ComponentModel.DataAnnotations;

namespace TrelloMini.Api.Models
{
    public class BoardTemplate
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "General"; // e.g., "Project Management", "Personal", "Marketing", etc.

        public bool IsPublic { get; set; } = true; // Public templates available to all users

        // User who created this template
        public int? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Template structure
        public ICollection<TemplateList> Lists { get; set; } = new List<TemplateList>();

        // Usage statistics
        public int UsageCount { get; set; } = 0; // How many times this template has been used
    }

    public class TemplateList
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public int Position { get; set; }

        public int TemplateId { get; set; }
        public BoardTemplate Template { get; set; } = null!;

        public ICollection<TemplateCard> Cards { get; set; } = new List<TemplateCard>();
    }

    public class TemplateCard
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public int Position { get; set; }

        public CardPriority Priority { get; set; } = CardPriority.None;

        public int TemplateListId { get; set; }
        public TemplateList TemplateList { get; set; } = null!;
    }
}
