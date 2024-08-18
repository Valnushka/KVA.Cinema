using KVA.Cinema.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace KVA.Cinema.Models
{
    public class CinemaContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<CommentMark> CommentMarks { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Director> Directors { get; set; }

        public virtual DbSet<Frame> Frames { get; set; }

        public virtual DbSet<Genre> Genres { get; set; }

        public virtual DbSet<Language> Languages { get; set; }

        public virtual DbSet<SubscriptionLevel> SubscriptionLevels { get; set; }

        public virtual DbSet<Pegi> Pegis { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<Subtitle> Subtitles { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<FileModel> FileModels { get; set; }

        //public override DbSet<Entities.User> Users { get; set; }

        public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }

        public virtual DbSet<Video> Videos { get; set; }

        public virtual DbSet<VideoInSubscription> VideoInSubscriptions { get; set; }

        public virtual DbSet<VideoRate> VideoRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Subscription>()
                .Property(p => p.Cost)
                .HasColumnType("decimal(18,4)");

            modelBuilder
                .Entity<Subtitle>() //каждый субтитр относится к одному конкретному видео
                .HasOne(e => e.Video) //связь один к одному субтитра с видео
                .WithMany(e => e.Subtitles) //у видео же может быть много субтитров (связь один ко многим)
                .OnDelete(DeleteBehavior.NoAction); //при удалении записи ничего не делать

            modelBuilder
                .Entity<CommentMark>()
                .HasOne(e => e.Comment)
                .WithMany(e => e.CommentMarks)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<IdentityUser>()
                .ToTable("Users", "dbo");
        }

        public CinemaContext(DbContextOptions<CinemaContext> options)
            : base(options)
        {
        }
    }
}
