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

    public virtual DbSet<TblAuditLog> TblAuditLogs { get; set; }

    public virtual DbSet<TblEvent> TblEvents { get; set; }

    public virtual DbSet<TblEventAdmin> TblEventAdmins { get; set; }

    public virtual DbSet<TblEventUser> TblEventUsers { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblProject> TblProjects { get; set; }

    public virtual DbSet<TblProjectType> TblProjectTypes { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserVoteLimit> TblUserVoteLimits { get; set; }

    public virtual DbSet<TblVote> TblVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Audi__3213E83F3D8C4714");

            entity.ToTable("Tbl_AuditLog");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("action");
            entity.Property(e => e.ActionDetails).HasColumnName("action_details");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("create_time");
            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.TableName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("table_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.TblAuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");
        });

        modelBuilder.Entity<TblEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Even__3213E83FE84761E5");

            entity.ToTable("Tbl_Event");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.EventName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("event_name");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<TblEventAdmin>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.AdminId }).HasName("PK__Tbl_Even__274A53335E45FC85");

            entity.ToTable("Tbl_EventAdmin");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");

            entity.HasOne(d => d.Admin).WithMany(p => p.TblEventAdmins)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventAdmin_User");

            entity.HasOne(d => d.Event).WithMany(p => p.TblEventAdmins)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventAdmin_Event");
        });

        modelBuilder.Entity<TblEventUser>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.UserId }).HasName("PK__Tbl_Even__C8EB14575547B596");

            entity.ToTable("Tbl_EventUser");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");

            entity.HasOne(d => d.Event).WithMany(p => p.TblEventUsers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_Event");

            entity.HasOne(d => d.User).WithMany(p => p.TblEventUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventUser_User");
        });

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Perm__3213E83F3875B718");

            entity.ToTable("Tbl_Permission");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("permission_name");
        });

        modelBuilder.Entity<TblProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Proj__3213E83FEB05B58F");

            entity.ToTable("Tbl_Project");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");

            entity.HasOne(d => d.Event).WithMany(p => p.TblProjects)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_Event");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblProjects)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Project_ProjectType");
        });

        modelBuilder.Entity<TblProjectType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Proj__3213E83FD8593E30");

            entity.ToTable("Tbl_ProjectType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.TypeName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Role__3213E83F2CE4E5EA");

            entity.ToTable("Tbl_Role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "TblRolePermission",
                    r => r.HasOne<TblPermission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermission_Permission"),
                    l => l.HasOne<TblRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_RolePermission_Role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__Tbl_Role__C85A54633AEB6D0C");
                        j.ToTable("Tbl_RolePermission");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_User__3213E83F45E70DA9");

            entity.ToTable("Tbl_User");

            entity.HasIndex(e => e.Email, "UQ__Tbl_User__AB6E616493D9D00D").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Tbl_User__F3DBC57279CEF857").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<TblUserVoteLimit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_User__3213E83F8EF809E5");

            entity.ToTable("Tbl_UserVoteLimit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.MaxVotes).HasColumnName("max_votes");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");
            entity.Property(e => e.UsedVotes)
                .HasDefaultValue(0)
                .HasColumnName("used_votes");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.TblUserVoteLimits)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserVoteLimit_Event");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblUserVoteLimits)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserVoteLimit_ProjectType");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserVoteLimits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserVoteLimit_User");
        });

        modelBuilder.Entity<TblVote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Vote__3213E83FCF47B4D6");

            entity.ToTable("Tbl_Vote");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateBy).HasColumnName("create_by");
            entity.Property(e => e.CreateTime).HasColumnName("create_time");
            entity.Property(e => e.DeleteFlag)
                .HasDefaultValue(false)
                .HasColumnName("delete_flag");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ModifyBy).HasColumnName("modify_by");
            entity.Property(e => e.ModifyTime).HasColumnName("modify_time");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Event).WithMany(p => p.TblVotes)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Event");

            entity.HasOne(d => d.Project).WithMany(p => p.TblVotes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_Project");

            entity.HasOne(d => d.User).WithMany(p => p.TblVotes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vote_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
