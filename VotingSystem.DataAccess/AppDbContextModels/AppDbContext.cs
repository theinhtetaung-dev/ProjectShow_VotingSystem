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

    public virtual DbSet<TblEmailAccount> TblEmailAccounts { get; set; }

    public virtual DbSet<TblEmailRoundRobinState> TblEmailRoundRobinStates { get; set; }

    public virtual DbSet<TblEvent> TblEvents { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblProject> TblProjects { get; set; }

    public virtual DbSet<TblProjectType> TblProjectTypes { get; set; }

    public virtual DbSet<TblRefreshToken> TblRefreshTokens { get; set; }

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
            entity.ToTable("Tbl_Admin_Event");

            entity.HasIndex(e => e.AdminUserId, "IX_Tbl_Admin_Event_AdminUserId");

            entity.HasIndex(e => e.EventId, "IX_Tbl_Admin_Event_EventId");

            entity.HasIndex(e => new { e.AdminUserId, e.EventId }, "UQ_Tbl_AdminEvent").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminUserId).HasColumnName("admin_user_id");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");

            entity.HasOne(d => d.AdminUser).WithMany(p => p.TblAdminEventAdminUsers)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdminEvent_User");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblAdminEventAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_Tbl_AdminEvent_AssignedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblAdminEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdminEvent_Event");
        });

        modelBuilder.Entity<TblAuditLog>(entity =>
        {
            entity.ToTable("Tbl_Audit_Log");

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "IX_Tbl_Audit_Log_Entity");

            entity.HasIndex(e => e.PerformedBy, "IX_Tbl_Audit_Log_PerformedBy");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.NewValue).HasColumnName("new_value");
            entity.Property(e => e.OldValue).HasColumnName("old_value");
            entity.Property(e => e.PerformedAt).HasColumnName("performed_at");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.TblAuditLogs)
                .HasForeignKey(d => d.PerformedBy)
                .HasConstraintName("FK_Tbl_AuditLog_PerformedBy");
        });

        modelBuilder.Entity<TblEmailAccount>(entity =>
        {
            entity.ToTable("Tbl_Email_Account");

            entity.HasIndex(e => new { e.IsActive, e.DeleteFlag, e.PriorityOrder, e.Id }, "IX_Tbl_Email_Account_ActiveSelection");

            entity.HasIndex(e => e.EmailAddress, "UQ_Tbl_Email_Account_EmailAddress")
                .IsUnique()
                .HasFilter("([delete_flag]=(0))");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(150)
                .HasColumnName("display_name");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(255)
                .HasColumnName("email_address");
            entity.Property(e => e.EncryptedAppPassword)
                .HasMaxLength(2000)
                .HasColumnName("encrypted_app_password");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.PriorityOrder).HasColumnName("priority_order");
            entity.Property(e => e.TotalEmailsSent).HasColumnName("total_emails_sent");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblEmailAccountCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Tbl_EmailAccount_CreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblEmailAccountModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Tbl_EmailAccount_ModifiedBy");
        });

        modelBuilder.Entity<TblEmailRoundRobinState>(entity =>
        {
            entity.ToTable("Tbl_Email_Round_Robin_State");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.LastSelectedAccountId).HasColumnName("last_selected_account_id");
            entity.Property(e => e.LastSelectedOrder).HasColumnName("last_selected_order");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(d => d.LastSelectedAccount).WithMany(p => p.TblEmailRoundRobinStates)
                .HasForeignKey(d => d.LastSelectedAccountId)
                .HasConstraintName("FK_Tbl_EmailRoundRobinState_Account");
        });

        modelBuilder.Entity<TblEvent>(entity =>
        {
            entity.ToTable("Tbl_Event");

            entity.HasIndex(e => e.Code, "UQ_Tbl_Event_Code").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EventEndAt).HasColumnName("event_end_at");
            entity.Property(e => e.EventStartAt).HasColumnName("event_start_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.ResultShowEndAt).HasColumnName("result_show_end_at");
            entity.Property(e => e.ResultShowStartAt).HasColumnName("result_show_start_at");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.VotingEndAt).HasColumnName("voting_end_at");
            entity.Property(e => e.VotingStartAt).HasColumnName("voting_start_at");
        });

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.ToTable("Tbl_Permission");

            entity.HasIndex(e => e.Code, "UQ_Tbl_Permission_Code").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblProject>(entity =>
        {
            entity.ToTable("Tbl_Project");

            entity.HasIndex(e => e.EventId, "IX_Tbl_Project_EventId");

            entity.HasIndex(e => e.ProjectTypeId, "IX_Tbl_Project_ProjectTypeId");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");
            entity.Property(e => e.VoteCounts).HasColumnName("vote_counts");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblProjectCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Tbl_Project_CreatedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblProjects)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Project_Event");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblProjectModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Tbl_Project_ModifiedBy");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblProjects)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Project_Type");
        });

        modelBuilder.Entity<TblProjectType>(entity =>
        {
            entity.ToTable("Tbl_Project_Type");

            entity.HasIndex(e => e.Code, "UQ_Tbl_ProjectType_Code").IsUnique();

            entity.HasIndex(e => e.Name, "UQ_Tbl_ProjectType_Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblRefreshToken>(entity =>
        {
            entity.ToTable("Tbl_Refresh_Token");

            entity.HasIndex(e => new { e.UserId, e.ExpiresAt, e.RevokedAt }, "IX_Tbl_Refresh_Token_ActiveLookup");

            entity.HasIndex(e => e.TokenFamilyId, "IX_Tbl_Refresh_Token_FamilyId");

            entity.HasIndex(e => e.UserId, "IX_Tbl_Refresh_Token_UserId");

            entity.HasIndex(e => e.TokenHash, "UQ_Tbl_Refresh_Token_Hash").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(45)
                .HasColumnName("created_by_ip");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(200)
                .HasColumnName("device_name");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(e => e.ReplacedByTokenId).HasColumnName("replaced_by_token_id");
            entity.Property(e => e.RevocationReason)
                .HasMaxLength(255)
                .HasColumnName("revocation_reason");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.RevokedByIp)
                .HasMaxLength(45)
                .HasColumnName("revoked_by_ip");
            entity.Property(e => e.TokenFamilyId).HasColumnName("token_family_id");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("token_hash");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ReplacedByToken).WithMany(p => p.InverseReplacedByToken)
                .HasForeignKey(d => d.ReplacedByTokenId)
                .HasConstraintName("FK_Tbl_RefreshToken_ReplacedBy");

            entity.HasOne(d => d.User).WithMany(p => p.TblRefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RefreshToken_User");
        });

        modelBuilder.Entity<TblResult>(entity =>
        {
            entity.ToTable("Tbl_Result");

            entity.HasIndex(e => e.EventId, "IX_Tbl_Result_EventId");

            entity.HasIndex(e => e.ProjectId, "IX_Tbl_Result_ProjectId");

            entity.HasIndex(e => new { e.EventId, e.ProjectId, e.Version }, "UQ_Tbl_Result").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.PublishedAt).HasColumnName("published_at");
            entity.Property(e => e.PublishedBy).HasColumnName("published_by");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.Version).HasColumnName("version");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblResultCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Tbl_Result_CreatedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblResults)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Result_Event");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblResultModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Tbl_Result_ModifiedBy");

            entity.HasOne(d => d.Project).WithMany(p => p.TblResults)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Result_Project");

            entity.HasOne(d => d.PublishedByNavigation).WithMany(p => p.TblResultPublishedByNavigations)
                .HasForeignKey(d => d.PublishedBy)
                .HasConstraintName("FK_Tbl_Result_PublishedBy");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.TblResultReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK_Tbl_Result_ReviewedBy");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("Tbl_Role");

            entity.HasIndex(e => e.Name, "UQ_Tbl_Role_Name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TblRolePermission>(entity =>
        {
            entity.ToTable("Tbl_Role_Permission");

            entity.HasIndex(e => new { e.RoleId, e.PermissionId }, "UQ_Tbl_RolePermission").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblRolePermissions)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_Tbl_RolePermission_AssignedBy");

            entity.HasOne(d => d.Permission).WithMany(p => p.TblRolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RolePermission_Permission");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RolePermission_Role");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.ToTable("Tbl_User");

            entity.HasIndex(e => e.RoleId, "IX_Tbl_User_RoleId");

            entity.HasIndex(e => e.Email, "UQ_Tbl_User_Email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_User_Role");
        });

        modelBuilder.Entity<TblUserEvent>(entity =>
        {
            entity.ToTable("Tbl_User_Event");

            entity.HasIndex(e => e.EventId, "IX_Tbl_User_Event_EventId");

            entity.HasIndex(e => e.UserId, "IX_Tbl_User_Event_UserId");

            entity.HasIndex(e => new { e.UserId, e.EventId }, "UQ_Tbl_UserEvent").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccessStatus)
                .HasMaxLength(20)
                .HasColumnName("access_status");
            entity.Property(e => e.AssignedAt).HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EventPasswordHash)
                .HasMaxLength(500)
                .HasColumnName("event_password_hash");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.PasswordExpiresAt).HasColumnName("password_expires_at");
            entity.Property(e => e.PasswordGeneratedAt).HasColumnName("password_generated_at");
            entity.Property(e => e.RemovedAt).HasColumnName("removed_at");
            entity.Property(e => e.RemovedBy).HasColumnName("removed_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.TblUserEventAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_Tbl_UserEvent_AssignedBy");

            entity.HasOne(d => d.Event).WithMany(p => p.TblUserEvents)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserEvent_Event");

            entity.HasOne(d => d.RemovedByNavigation).WithMany(p => p.TblUserEventRemovedByNavigations)
                .HasForeignKey(d => d.RemovedBy)
                .HasConstraintName("FK_Tbl_UserEvent_RemovedBy");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserEventUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserEvent_User");
        });

        modelBuilder.Entity<TblVote>(entity =>
        {
            entity.ToTable("Tbl_Vote");

            entity.HasIndex(e => e.ProjectId, "IX_Tbl_Vote_ProjectId");

            entity.HasIndex(e => e.UserEventId, "IX_Tbl_Vote_UserEventId");

            entity.HasIndex(e => new { e.UserEventId, e.ProjectId }, "UQ_Tbl_Vote_UserEvent_Project").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.UserEventId).HasColumnName("user_event_id");
            entity.Property(e => e.VotedAt).HasColumnName("voted_at");

            entity.HasOne(d => d.Project).WithMany(p => p.TblVotes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Vote_Project");

            entity.HasOne(d => d.UserEvent).WithMany(p => p.TblVotes)
                .HasForeignKey(d => d.UserEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Vote_UserEvent");
        });

        modelBuilder.Entity<TblVotingLimit>(entity =>
        {
            entity.ToTable("Tbl_Voting_Limit");

            entity.HasIndex(e => e.UserEventId, "IX_Tbl_Voting_Limit_UserEventId");

            entity.HasIndex(e => new { e.UserEventId, e.ProjectTypeId }, "UQ_Tbl_VotingLimit").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeleteFlag).HasColumnName("delete_flag");
            entity.Property(e => e.ModifiedAt).HasColumnName("modified_at");
            entity.Property(e => e.ModifiedBy).HasColumnName("modified_by");
            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");
            entity.Property(e => e.UserEventId).HasColumnName("user_event_id");
            entity.Property(e => e.VoteLimit).HasColumnName("vote_limit");
            entity.Property(e => e.VoteUsed).HasColumnName("vote_used");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblVotingLimitCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Tbl_VotingLimit_CreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.TblVotingLimitModifiedByNavigations)
                .HasForeignKey(d => d.ModifiedBy)
                .HasConstraintName("FK_Tbl_VotingLimit_ModifiedBy");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.TblVotingLimits)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_VotingLimit_ProjectType");

            entity.HasOne(d => d.UserEvent).WithMany(p => p.TblVotingLimits)
                .HasForeignKey(d => d.UserEventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_VotingLimit_UserEvent");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
