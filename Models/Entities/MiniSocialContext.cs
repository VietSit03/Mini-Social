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

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserOauth> UserOauths { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FriendRe__3214EC0747F9860F");

            entity.HasIndex(e => new { e.ReceiverId, e.Status }, "IX_FriendRequests_ReceiverId");

            entity.HasIndex(e => e.ReceiverId, "IX_FriendRequests_Receiver_Pending").HasFilter("([Status]='P')");

            entity.HasIndex(e => new { e.SenderId, e.Status }, "IX_FriendRequests_SenderId");

            entity.HasIndex(e => new { e.SenderId, e.ReceiverId }, "IX_FriendRequests_SenderReceiver");

            entity.HasIndex(e => e.SenderId, "IX_FriendRequests_Sender_Pending").HasFilter("([Status]='P')");

            entity.HasIndex(e => new { e.Status, e.SenderId, e.ReceiverId }, "IX_FriendRequests_Status_Sender_Receiver");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendRequestReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_Receiver");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendRequestSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_Sender");
        });

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
