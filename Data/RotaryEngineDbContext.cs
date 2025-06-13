using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using rotaryproject.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace rotaryproject.Data
{
    public partial class RotaryEngineDbContext : IdentityDbContext<ApplicationUser>
    {
        public RotaryEngineDbContext(DbContextOptions<RotaryEngineDbContext> options)
            : base(options)
        {
        }

        // DbSets for all your application's tables
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PartCategory> PartCategories { get; set; }
        public virtual DbSet<UserSavedBuild> UserSavedBuilds { get; set; }
        public virtual DbSet<EngineFamily> EngineFamilies { get; set; }
        public virtual DbSet<PartFitment> PartFitments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<PostRendition> PostRenditions { get; set; }
        public virtual DbSet<UserFollow> UserFollows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This is required and must be called first.
            base.OnModelCreating(modelBuilder); 

            // --- Compatibility System Configuration ---
            // This is the correct place for many-to-many configuration.
            modelBuilder.Entity<PartFitment>()
                .HasKey(pf => new { pf.PartId, pf.EngineFamilyId });

            modelBuilder.Entity<PartFitment>()
                .HasOne(pf => pf.Part)
                .WithMany(p => p.Fitments)
                .HasForeignKey(pf => pf.PartId);

            modelBuilder.Entity<PartFitment>()
                .HasOne(pf => pf.EngineFamily)
                .WithMany(ef => ef.PartFitments)
                .HasForeignKey(pf => pf.EngineFamilyId);

            
            // --- UserSavedBuild Configuration ---
            modelBuilder.Entity<UserSavedBuild>(entity =>
            {
                entity.HasKey(e => e.UserSavedBuildId);
                entity.Property(e => e.BuildName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BuildConfigurationString).IsRequired();
                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                      .WithMany(p => p.SavedBuilds)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // --- Social Feature Configurations ---
            
            // Post and PostRendition relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostRendition>()
                .HasOne(pr => pr.Post)
                .WithMany(p => p.Renditions)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserFollow (Follower/Following) many-to-many relationship
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);
            // The redundant configurations for Part and PartCategory have been removed,
            // as they are handled by the attributes in the model files.
        }
    }
}
