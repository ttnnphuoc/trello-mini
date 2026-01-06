using Microsoft.EntityFrameworkCore;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Data
{
    public class TrelloDbContext : DbContext
    {
        public TrelloDbContext(DbContextOptions<TrelloDbContext> options) : base(options)
        {
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasMany(e => e.Lists)
                    .WithOne(e => e.Board)
                    .HasForeignKey(e => e.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Board)
                    .WithMany(e => e.Lists)
                    .HasForeignKey(e => e.BoardId);
                entity.HasMany(e => e.Cards)
                    .WithOne(e => e.List)
                    .HasForeignKey(e => e.ListId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasOne(e => e.List)
                    .WithMany(e => e.Cards)
                    .HasForeignKey(e => e.ListId);
            });
        }
    }
}