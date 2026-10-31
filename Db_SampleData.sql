/* ============================================================
   VOTING SYSTEM - SAMPLE DATA
   Run after Db_V2.sql and Db_V3.sql.
   Safe to run more than once: records are matched by natural keys.
   ============================================================ */

USE VotingSystemDb;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* Roles */
IF NOT EXISTS (SELECT 1 FROM Tbl_Role WHERE name = N'SuperAdmin')
    INSERT INTO Tbl_Role (name, description, is_active, delete_flag, created_at)
    VALUES (N'SuperAdmin', N'Full system access', 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Role WHERE name = N'Admin')
    INSERT INTO Tbl_Role (name, description, is_active, delete_flag, created_at)
    VALUES (N'Admin', N'Event management access', 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Role WHERE name = N'User')
    INSERT INTO Tbl_Role (name, description, is_active, delete_flag, created_at)
    VALUES (N'User', N'Voting access', 1, 0, SYSUTCDATETIME());

/* Permissions */
IF NOT EXISTS (SELECT 1 FROM Tbl_Permission WHERE code = N'MANAGE_EVENTS')
    INSERT INTO Tbl_Permission (name, code, description, is_active, delete_flag, created_at)
    VALUES (N'Manage Events', N'MANAGE_EVENTS', N'Create, edit, and delete events', 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Permission WHERE code = N'MANAGE_PROJECTS')
    INSERT INTO Tbl_Permission (name, code, description, is_active, delete_flag, created_at)
    VALUES (N'Manage Projects', N'MANAGE_PROJECTS', N'Create, edit, and delete projects', 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Permission WHERE code = N'CAST_VOTE')
    INSERT INTO Tbl_Permission (name, code, description, is_active, delete_flag, created_at)
    VALUES (N'Cast Vote', N'CAST_VOTE', N'Vote for projects in an assigned event', 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Permission WHERE code = N'VIEW_RESULTS')
    INSERT INTO Tbl_Permission (name, code, description, is_active, delete_flag, created_at)
    VALUES (N'View Results', N'VIEW_RESULTS', N'View published event results', 1, 0, SYSUTCDATETIME());

/* Project types */
IF NOT EXISTS (SELECT 1 FROM Tbl_Project_Type WHERE code = N'HARDWARE')
    INSERT INTO Tbl_Project_Type (name, code, is_active, delete_flag)
    VALUES (N'Hardware', N'HARDWARE', 1, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_Project_Type WHERE code = N'SOFTWARE')
    INSERT INTO Tbl_Project_Type (name, code, is_active, delete_flag)
    VALUES (N'Software', N'SOFTWARE', 1, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_Project_Type WHERE code = N'SOCIAL_IMPACT')
    INSERT INTO Tbl_Project_Type (name, code, is_active, delete_flag)
    VALUES (N'Social Impact', N'SOCIAL_IMPACT', 1, 0);

DECLARE @AdminRoleId INT = (SELECT TOP 1 id FROM Tbl_Role WHERE name = N'Admin' AND delete_flag = 0);
DECLARE @UserRoleId INT = (SELECT TOP 1 id FROM Tbl_Role WHERE name = N'User' AND delete_flag = 0);

/* Password hashes are intentionally NULL. Create credentials through the API before testing login. */
IF NOT EXISTS (SELECT 1 FROM Tbl_User WHERE email = N'admin.demo@voting.local')
    INSERT INTO Tbl_User (email, name, password_hash, role_id, is_active, delete_flag, created_at)
    VALUES (N'admin.demo@voting.local', N'Demo Administrator', NULL, @AdminRoleId, 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_User WHERE email = N'alice.demo@voting.local')
    INSERT INTO Tbl_User (email, name, password_hash, role_id, is_active, delete_flag, created_at)
    VALUES (N'alice.demo@voting.local', N'Alice Voter', NULL, @UserRoleId, 1, 0, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_User WHERE email = N'bob.demo@voting.local')
    INSERT INTO Tbl_User (email, name, password_hash, role_id, is_active, delete_flag, created_at)
    VALUES (N'bob.demo@voting.local', N'Bob Voter', NULL, @UserRoleId, 1, 0, SYSUTCDATETIME());

DECLARE @AdminUserId BIGINT = (SELECT id FROM Tbl_User WHERE email = N'admin.demo@voting.local');
DECLARE @AliceUserId BIGINT = (SELECT id FROM Tbl_User WHERE email = N'alice.demo@voting.local');
DECLARE @BobUserId BIGINT = (SELECT id FROM Tbl_User WHERE email = N'bob.demo@voting.local');

/* Role permissions */
INSERT INTO Tbl_Role_Permission (role_id, permission_id, assigned_at, assigned_by, delete_flag)
SELECT @AdminRoleId, p.id, SYSUTCDATETIME(), @AdminUserId, 0
FROM Tbl_Permission p
WHERE p.code IN (N'MANAGE_EVENTS', N'MANAGE_PROJECTS', N'VIEW_RESULTS')
  AND NOT EXISTS
      (SELECT 1 FROM Tbl_Role_Permission rp WHERE rp.role_id = @AdminRoleId AND rp.permission_id = p.id);

INSERT INTO Tbl_Role_Permission (role_id, permission_id, assigned_at, assigned_by, delete_flag)
SELECT @UserRoleId, p.id, SYSUTCDATETIME(), @AdminUserId, 0
FROM Tbl_Permission p
WHERE p.code IN (N'CAST_VOTE', N'VIEW_RESULTS')
  AND NOT EXISTS
      (SELECT 1 FROM Tbl_Role_Permission rp WHERE rp.role_id = @UserRoleId AND rp.permission_id = p.id);

/* Events: dates remain useful whenever the script is run. */
IF NOT EXISTS (SELECT 1 FROM Tbl_Event WHERE code = N'DEMO-ACTIVE')
    INSERT INTO Tbl_Event
        (name, code, description, event_start_at, event_end_at, voting_start_at, voting_end_at,
         result_show_start_at, result_show_end_at, status, is_active, delete_flag, created_at, created_by)
    VALUES
        (N'Demo Innovation Challenge', N'DEMO-ACTIVE', N'Active sample event for testing the MVC application.',
         DATEADD(DAY, -2, SYSUTCDATETIME()), DATEADD(DAY, 5, SYSUTCDATETIME()),
         DATEADD(DAY, -1, SYSUTCDATETIME()), DATEADD(DAY, 4, SYSUTCDATETIME()),
         DATEADD(DAY, 4, SYSUTCDATETIME()), DATEADD(DAY, 7, SYSUTCDATETIME()),
         N'Active', 1, 0, SYSUTCDATETIME(), @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM Tbl_Event WHERE code = N'DEMO-UPCOMING')
    INSERT INTO Tbl_Event
        (name, code, description, event_start_at, event_end_at, voting_start_at, voting_end_at,
         result_show_start_at, result_show_end_at, status, is_active, delete_flag, created_at, created_by)
    VALUES
        (N'Future Tech Showcase', N'DEMO-UPCOMING', N'Upcoming sample event.',
         DATEADD(DAY, 10, SYSUTCDATETIME()), DATEADD(DAY, 17, SYSUTCDATETIME()),
         DATEADD(DAY, 11, SYSUTCDATETIME()), DATEADD(DAY, 16, SYSUTCDATETIME()),
         DATEADD(DAY, 16, SYSUTCDATETIME()), DATEADD(DAY, 19, SYSUTCDATETIME()),
         N'Upcoming', 1, 0, SYSUTCDATETIME(), @AdminUserId);

DECLARE @EventId BIGINT = (SELECT id FROM Tbl_Event WHERE code = N'DEMO-ACTIVE');
DECLARE @HardwareTypeId INT = (SELECT id FROM Tbl_Project_Type WHERE code = N'HARDWARE');
DECLARE @SoftwareTypeId INT = (SELECT id FROM Tbl_Project_Type WHERE code = N'SOFTWARE');
DECLARE @SocialTypeId INT = (SELECT id FROM Tbl_Project_Type WHERE code = N'SOCIAL_IMPACT');

/* Event administrator and voters */
IF NOT EXISTS (SELECT 1 FROM Tbl_Admin_Event WHERE admin_user_id = @AdminUserId AND event_id = @EventId)
    INSERT INTO Tbl_Admin_Event (admin_user_id, event_id, assigned_at, assigned_by, is_active, delete_flag)
    VALUES (@AdminUserId, @EventId, SYSUTCDATETIME(), @AdminUserId, 1, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_User_Event WHERE user_id = @AliceUserId AND event_id = @EventId)
    INSERT INTO Tbl_User_Event
        (user_id, event_id, event_password_hash, assigned_at, assigned_by, password_generated_at,
         password_expires_at, access_status, is_active, delete_flag)
    VALUES
        (@AliceUserId, @EventId, N'DEMO-NOT-FOR-LOGIN', SYSUTCDATETIME(), @AdminUserId,
         SYSUTCDATETIME(), DATEADD(DAY, 5, SYSUTCDATETIME()), N'valid', 1, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_User_Event WHERE user_id = @BobUserId AND event_id = @EventId)
    INSERT INTO Tbl_User_Event
        (user_id, event_id, event_password_hash, assigned_at, assigned_by, password_generated_at,
         password_expires_at, access_status, is_active, delete_flag)
    VALUES
        (@BobUserId, @EventId, N'DEMO-NOT-FOR-LOGIN', SYSUTCDATETIME(), @AdminUserId,
         SYSUTCDATETIME(), DATEADD(DAY, 5, SYSUTCDATETIME()), N'valid', 1, 0);

/* Projects */
IF NOT EXISTS (SELECT 1 FROM Tbl_Project WHERE event_id = @EventId AND name = N'Smart Recycling Station')
    INSERT INTO Tbl_Project (event_id, project_type_id, name, description, vote_counts, is_active, delete_flag, created_at, created_by)
    VALUES (@EventId, @HardwareTypeId, N'Smart Recycling Station', N'AI-assisted sorting station for community recycling.', 0, 1, 0, SYSUTCDATETIME(), @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM Tbl_Project WHERE event_id = @EventId AND name = N'Community Health Portal')
    INSERT INTO Tbl_Project (event_id, project_type_id, name, description, vote_counts, is_active, delete_flag, created_at, created_by)
    VALUES (@EventId, @SoftwareTypeId, N'Community Health Portal', N'A lightweight portal connecting residents with local health services.', 0, 1, 0, SYSUTCDATETIME(), @AdminUserId);

IF NOT EXISTS (SELECT 1 FROM Tbl_Project WHERE event_id = @EventId AND name = N'Accessible Learning Network')
    INSERT INTO Tbl_Project (event_id, project_type_id, name, description, vote_counts, is_active, delete_flag, created_at, created_by)
    VALUES (@EventId, @SocialTypeId, N'Accessible Learning Network', N'Peer-supported learning resources for underserved students.', 0, 1, 0, SYSUTCDATETIME(), @AdminUserId);

DECLARE @AliceUserEventId BIGINT = (SELECT id FROM Tbl_User_Event WHERE user_id = @AliceUserId AND event_id = @EventId);
DECLARE @BobUserEventId BIGINT = (SELECT id FROM Tbl_User_Event WHERE user_id = @BobUserId AND event_id = @EventId);
DECLARE @HardwareProjectId BIGINT = (SELECT id FROM Tbl_Project WHERE event_id = @EventId AND name = N'Smart Recycling Station');
DECLARE @SoftwareProjectId BIGINT = (SELECT id FROM Tbl_Project WHERE event_id = @EventId AND name = N'Community Health Portal');
DECLARE @SocialProjectId BIGINT = (SELECT id FROM Tbl_Project WHERE event_id = @EventId AND name = N'Accessible Learning Network');

/* Voting limits */
INSERT INTO Tbl_Voting_Limit (user_event_id, project_type_id, vote_limit, vote_used, created_at, created_by, delete_flag)
SELECT ue.id, pt.id, 2, 0, SYSUTCDATETIME(), @AdminUserId, 0
FROM (VALUES (@AliceUserEventId), (@BobUserEventId)) ue(id)
CROSS JOIN (VALUES (@HardwareTypeId), (@SoftwareTypeId), (@SocialTypeId)) pt(id)
WHERE NOT EXISTS
      (SELECT 1 FROM Tbl_Voting_Limit vl WHERE vl.user_event_id = ue.id AND vl.project_type_id = pt.id);

/* Example votes */
IF NOT EXISTS (SELECT 1 FROM Tbl_Vote WHERE user_event_id = @AliceUserEventId AND project_id = @HardwareProjectId)
    INSERT INTO Tbl_Vote (user_event_id, project_id, voted_at) VALUES (@AliceUserEventId, @HardwareProjectId, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Vote WHERE user_event_id = @AliceUserEventId AND project_id = @SoftwareProjectId)
    INSERT INTO Tbl_Vote (user_event_id, project_id, voted_at) VALUES (@AliceUserEventId, @SoftwareProjectId, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Tbl_Vote WHERE user_event_id = @BobUserEventId AND project_id = @SocialProjectId)
    INSERT INTO Tbl_Vote (user_event_id, project_id, voted_at) VALUES (@BobUserEventId, @SocialProjectId, SYSUTCDATETIME());

UPDATE p
SET vote_counts = (SELECT COUNT(*) FROM Tbl_Vote v WHERE v.project_id = p.id)
FROM Tbl_Project p
WHERE p.id IN (@HardwareProjectId, @SoftwareProjectId, @SocialProjectId);

UPDATE vl
SET vote_used =
    (SELECT COUNT(*)
     FROM Tbl_Vote v
     INNER JOIN Tbl_Project p ON p.id = v.project_id
     WHERE v.user_event_id = vl.user_event_id AND p.project_type_id = vl.project_type_id)
FROM Tbl_Voting_Limit vl
WHERE vl.user_event_id IN (@AliceUserEventId, @BobUserEventId);

/* Results */
IF NOT EXISTS (SELECT 1 FROM Tbl_Result WHERE event_id = @EventId AND project_id = @HardwareProjectId AND version = 1)
    INSERT INTO Tbl_Result
        (event_id, project_id, status, version, reviewed_at, reviewed_by, published_at, published_by,
         created_at, created_by, delete_flag)
    VALUES
        (@EventId, @HardwareProjectId, N'first_prize', 1, SYSUTCDATETIME(), @AdminUserId,
         SYSUTCDATETIME(), @AdminUserId, SYSUTCDATETIME(), @AdminUserId, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_Result WHERE event_id = @EventId AND project_id = @SoftwareProjectId AND version = 1)
    INSERT INTO Tbl_Result
        (event_id, project_id, status, version, reviewed_at, reviewed_by, published_at, published_by,
         created_at, created_by, delete_flag)
    VALUES
        (@EventId, @SoftwareProjectId, N'second_prize', 1, SYSUTCDATETIME(), @AdminUserId,
         SYSUTCDATETIME(), @AdminUserId, SYSUTCDATETIME(), @AdminUserId, 0);

IF NOT EXISTS (SELECT 1 FROM Tbl_Result WHERE event_id = @EventId AND project_id = @SocialProjectId AND version = 1)
    INSERT INTO Tbl_Result
        (event_id, project_id, status, version, reviewed_at, reviewed_by, published_at, published_by,
         created_at, created_by, delete_flag)
    VALUES
        (@EventId, @SocialProjectId, N'third_prize', 1, SYSUTCDATETIME(), @AdminUserId,
         SYSUTCDATETIME(), @AdminUserId, SYSUTCDATETIME(), @AdminUserId, 0);

COMMIT TRANSACTION;
GO

PRINT N'Sample data is ready.';
GO
