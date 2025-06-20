using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MiniSocialAPI.Models.Entities;

public partial class MiniSocialContext : DbContext
{
    public MiniSocialContext()
    {
    }

    public MiniSocialContext(DbContextOptions<MiniSocialContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOauth> UserOauths { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07DAF94328");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E423A3AC75").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105347B0A96C7").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.Bio).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserOauth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserOAut__3214EC079B6F7128");

            entity.ToTable("UserOAuths");

            entity.HasIndex(e => new { e.Provider, e.ProviderUserId }, "IX_UserOAuths_Provider_ProviderUserId").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Provider)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProviderUserId).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.UserOauths)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserOAuths_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
