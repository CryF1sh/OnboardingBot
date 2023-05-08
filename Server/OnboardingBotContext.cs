using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Entities;

namespace Server;

public partial class OnboardingBotContext : DbContext
{
    public OnboardingBotContext()
    {
    }

    public OnboardingBotContext(DbContextOptions<OnboardingBotContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cabinet> Cabinets { get; set; }

    public virtual DbSet<Direction> Directions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeDirection> EmployeeDirections { get; set; }

    public virtual DbSet<FloorLayout> FloorLayouts { get; set; }

    public virtual DbSet<UsefulLink> UsefulLinks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserQuestion> UserQuestions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OnboardingBot;Username=onboardingbot;Password=onboardingbot");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cabinet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Cabinet_pkey");

            entity.ToTable("Cabinet");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FloorLayoutId).HasColumnName("FloorLayoutID");
            entity.Property(e => e.Name).HasMaxLength(250);

            entity.HasOne(d => d.Employee).WithMany(p => p.Cabinets)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("Cabinet_EmployeeID_fkey");

            entity.HasOne(d => d.FloorLayout).WithMany(p => p.Cabinets)
                .HasForeignKey(d => d.FloorLayoutId)
                .HasConstraintName("Cabinet_FloorLayoutID_fkey");
        });

        modelBuilder.Entity<Direction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Direction_pkey");

            entity.ToTable("Direction");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Employee_pkey");

            entity.ToTable("Employee");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(100);
            entity.Property(e => e.Telephone).HasMaxLength(20);
        });

        modelBuilder.Entity<EmployeeDirection>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Employee_Direction");

            entity.Property(e => e.DirectionId).HasColumnName("Direction_id");
            entity.Property(e => e.EmployeeId).HasColumnName("Employee_id");

            entity.HasOne(d => d.Direction).WithMany()
                .HasForeignKey(d => d.DirectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Employee_Direction_Direction_id_fkey");

            entity.HasOne(d => d.Employee).WithMany()
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Employee_Direction_Employee_id_fkey");
        });

        modelBuilder.Entity<FloorLayout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("FloorLayout_pkey");

            entity.ToTable("FloorLayout");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(250);
        });

        modelBuilder.Entity<UsefulLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UsefulLink_pkey");

            entity.ToTable("UsefulLink");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DirectionId).HasColumnName("DirectionID");
            entity.Property(e => e.TelegramId).HasColumnName("TelegramID");

            entity.HasOne(d => d.Direction).WithMany(p => p.Users)
                .HasForeignKey(d => d.DirectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_DirectionID_fkey");
        });

        modelBuilder.Entity<UserQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserQuestion_pkey");

            entity.ToTable("UserQuestion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.UserQuestions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserQuestion_UserID_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
