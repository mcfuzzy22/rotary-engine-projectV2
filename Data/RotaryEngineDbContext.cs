using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using rotaryproject.Data.Models;
using Microsoft.AspNetCore.Identity; // Add this for IdentityUser (if not using ApplicationUser directly)
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Add this for IdentityDbContext
namespace rotaryproject.Data;

public partial class RotaryEngineDbContext : IdentityDbContext<ApplicationUser> // <<< MODIFIED HERE
{
    public RotaryEngineDbContext()
    {
    }

    public RotaryEngineDbContext(DbContextOptions<RotaryEngineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CompatibilityRule> CompatibilityRules { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartCategory> PartCategories { get; set; }

    public virtual DbSet<PartStat> PartStats { get; set; }
    public virtual DbSet<UserSavedBuild> UserSavedBuilds { get; set; }
   // In your RotaryEngineDbContext.cs

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // Intentionally left blank or only calls base.OnConfiguring if needed.
    // Since Program.cs configures the DbContext with the connection string,
    // we don't need to (and shouldn't) configure it here with a hardcoded string.
    // This will remove the CS1030 warning.

    // If you want to be explicit about not doing anything if already configured:
    // if (!optionsBuilder.IsConfigured)
    // {
    //     // This block would now typically be empty or log an error if somehow not configured by Program.cs,
    //     // but for most cases, an empty method is fine.
    // }
}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CompatibilityRule>(entity =>
        {
            entity.HasKey(e => e.RuleId).HasName("PK__Compatib__110458C2D19B7CFD");

            entity.Property(e => e.IsCompatible).HasDefaultValue(true);

            entity.HasOne(d => d.PartA).WithMany(p => p.CompatibilityRulePartAs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compatibi__PartA__4316F928");

            entity.HasOne(d => d.PartB).WithMany(p => p.CompatibilityRulePartBs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compatibi__PartB__440B1D61");
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__Parts__7C3F0D3095AB3286");

            entity.HasOne(d => d.Category).WithMany(p => p.Parts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Parts__CategoryI__3B75D760");
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__PartCate__19093A2B0B380B12");
        });

        modelBuilder.Entity<PartStat>(entity =>
        {
            entity.HasKey(e => e.PartStatId).HasName("PK__PartStat__AFCC32BA505FA52A");

            entity.HasOne(d => d.Part).WithMany(p => p.PartStats).HasConstraintName("FK__PartStats__PartI__3E52440B");
        });
        modelBuilder.Entity<UserSavedBuild>(entity =>
            {
                entity.HasKey(e => e.UserSavedBuildId);

                entity.Property(e => e.BuildName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BuildConfigurationString).IsRequired(); // Can be long, so no MaxLength by default
                entity.Property(e => e.UserId).IsRequired();

                // Define the relationship to ApplicationUser
                // An ApplicationUser can have many UserSavedBuilds
                // A UserSavedBuild belongs to one ApplicationUser
                entity.HasOne(d => d.User)
                      .WithMany(p => p.SavedBuilds) // Corresponds to ICollection in ApplicationUser
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Or DeleteBehavior.ClientSetNull if you prefer
            });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
