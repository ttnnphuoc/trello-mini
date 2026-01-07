using Microsoft.EntityFrameworkCore;
using TrelloMini.Api.Models;

namespace TrelloMini.Api.Data
{
    public class TrelloDbContext : DbContext
    {
        public TrelloDbContext(DbContextOptions<TrelloDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<BoardMember> BoardMembers { get; set; }
        public DbSet<BoardInvitation> BoardInvitations { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasMany(e => e.Boards)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.BoardMemberships)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.SentInvitations)
                    .WithOne(e => e.InvitedBy)
                    .HasForeignKey(e => e.InvitedByUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.ReceivedInvitations)
                    .WithOne(e => e.InvitedUser)
                    .HasForeignKey(e => e.InvitedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.Notifications)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Board>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasMany(e => e.Lists)
                    .WithOne(e => e.Board)
                    .HasForeignKey(e => e.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Members)
                    .WithOne(e => e.Board)
                    .HasForeignKey(e => e.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Invitations)
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

            modelBuilder.Entity<BoardMember>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.BoardId, e.UserId }).IsUnique();
                entity.Property(e => e.Role).IsRequired();
                entity.HasOne(e => e.InvitedBy)
                    .WithMany()
                    .HasForeignKey(e => e.InvitedByUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<BoardInvitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(64);
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => new { e.BoardId, e.Email }).IsUnique();
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.Data).HasMaxLength(2000);
                entity.HasOne(e => e.Board)
                    .WithMany()
                    .HasForeignKey(e => e.BoardId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Card)
                    .WithMany()
                    .HasForeignKey(e => e.CardId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.List)
                    .WithMany()
                    .HasForeignKey(e => e.ListId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ActorUser)
                    .WithMany()
                    .HasForeignKey(e => e.ActorUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}