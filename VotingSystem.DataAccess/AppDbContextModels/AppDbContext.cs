using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VotingSystem.DataAccess.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAdminEvent> TblAdminEvents { get; set; }

    public virtual DbSet<TblAuditLog> TblAuditLogs { get; set; }

    public virtual DbSet<TblEvent> TblEvents { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblProject> TblProjects { get; set; }

    public virtual DbSet<TblProjectType> TblProjectTypes { get; set; }

    public virtual DbSet<TblResult> TblResults { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRolePermission> TblRolePermissions { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserEvent> TblUserEvents { get; set; }

    public virtual DbSet<TblVote> TblVotes { get; set; }

    public virtual DbSet<TblVotingLimit> TblVotingLimits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAdminEvent>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.AdminUser).WithMany(p => p.TblAdminEventAdminUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdminEvent_User");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblAdminEventAssignedByNavigations).HasConstraintName("FK_Tbl_AdminEvent_AssignedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblAdminEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdminEvent_Event");
        });

        modelBuilder.Entity<TblAuditLog>(entity =>
        {
            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.TblAuditLogs).HasConstraintName("FK_Tbl_AuditLog_PerformedBy");
        });

        modelBuilder.Entity<TblEvent>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblProject>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblProjectCreatedByNavigations).HasConstraintName("FK_Tbl_Project_CreatedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblProjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Project_Event");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblProjectModifiedByNavigations).HasConstraintName("FK_Tbl_Project_ModifiedBy");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblProjects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Project_Type");
        });

        modelBuilder.Entity<TblProjectType>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblResult>(entity =>
        {
            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblResultCreatedByNavigations).HasConstraintName("FK_Tbl_Result_CreatedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Result_Event");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblResultModifiedByNavigations).HasConstraintName("FK_Tbl_Result_ModifiedBy");

            entity.HasOne(d => d.Project).WithMany(p => p.TblResults)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Result_Project");

            entity.HasOne(d => d.PublishedByNavigation).WithMany(p => p.TblResultPublishedByNavigations).HasConstraintName("FK_Tbl_Result_PublishedBy");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.TblResultReviewedByNavigations).HasConstraintName("FK_Tbl_Result_ReviewedBy");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblRolePermission>(entity =>
        {
            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblRolePermissions).HasConstraintName("FK_Tbl_RolePermission_AssignedBy");

            entity.HasOne(d => d.Permission).WithMany(p => p.TblRolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RolePermission_Role");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_User_Role");
        });

        modelBuilder.Entity<TblUserEvent>(entity =>
        {
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblUserEventAssignedByNavigations).HasConstraintName("FK_Tbl_UserEvent_AssignedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblUserEvents)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserEvent_Event");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.TblUserEventRemovedByNavigations).HasConstraintName("FK_Tbl_UserEvent_RemovedBy");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserEventUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserEvent_User");
        });

        modelBuilder.Entity<TblVote>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.TblVotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Vote_Project");

            entity.HasOne(d => d.UserEvent).WithMany(p => p.TblVotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Vote_UserEvent");
        });

        modelBuilder.Entity<TblVotingLimit>(entity =>
        {
            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblVotingLimitCreatedByNavigations).HasConstraintName("FK_Tbl_VotingLimit_CreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblVotingLimitModifiedByNavigations).HasConstraintName("FK_Tbl_VotingLimit_ModifiedBy");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblVotingLimits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_VotingLimit_ProjectType");

            entity.HasOne(d => d.UserEvent).WithMany(p => p.TblVotingLimits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_VotingLimit_UserEvent");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
