/* ============================================================
   VOTING SYSTEM DATABASE
   Database: Microsoft SQL Server
   Framework: .NET 8
   ============================================================ */


-- ============================================================
-- 1. CREATE DATABASE
-- ============================================================

IF DB_ID(N'VotingSystemDb') IS NULL
BEGIN
    CREATE DATABASE VotingSystemDb;
END
GO

USE VotingSystemDb;
GO


-- ============================================================
-- 2. DROP TABLES IF THEY EXIST
--    Use only for development/reset purposes.
-- ============================================================

IF OBJECT_ID(N'Tbl_Audit_Log', N'U') IS NOT NULL
    DROP TABLE Tbl_Audit_Log;

IF OBJECT_ID(N'Tbl_Result', N'U') IS NOT NULL
    DROP TABLE Tbl_Result;

IF OBJECT_ID(N'Tbl_Vote', N'U') IS NOT NULL
    DROP TABLE Tbl_Vote;

IF OBJECT_ID(N'Tbl_Voting_Limit', N'U') IS NOT NULL
    DROP TABLE Tbl_Voting_Limit;

IF OBJECT_ID(N'Tbl_Project', N'U') IS NOT NULL
    DROP TABLE Tbl_Project;

IF OBJECT_ID(N'Tbl_Project_Type', N'U') IS NOT NULL
    DROP TABLE Tbl_Project_Type;

IF OBJECT_ID(N'Tbl_User_Event', N'U') IS NOT NULL
    DROP TABLE Tbl_User_Event;

IF OBJECT_ID(N'Tbl_Admin_Event', N'U') IS NOT NULL
    DROP TABLE Tbl_Admin_Event;

IF OBJECT_ID(N'Tbl_Event', N'U') IS NOT NULL
    DROP TABLE Tbl_Event;

IF OBJECT_ID(N'Tbl_Role_Permission', N'U') IS NOT NULL
    DROP TABLE Tbl_Role_Permission;

IF OBJECT_ID(N'Tbl_Permission', N'U') IS NOT NULL
    DROP TABLE Tbl_Permission;

IF OBJECT_ID(N'Tbl_Role', N'U') IS NOT NULL
    DROP TABLE Tbl_Role;

IF OBJECT_ID(N'Tbl_User', N'U') IS NOT NULL
    DROP TABLE Tbl_User;
GO


-- ============================================================
-- 3. TBL_ROLE
-- ============================================================

CREATE TABLE Tbl_Role
(
    id INT IDENTITY(1,1) NOT NULL,

    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255) NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_Role_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_Role_DeleteFlag DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    CONSTRAINT PK_Tbl_Role
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_Role_Name
        UNIQUE (name)
);
GO


-- ============================================================
-- 4. TBL_PERMISSION
-- ============================================================

CREATE TABLE Tbl_Permission
(
    id INT IDENTITY(1,1) NOT NULL,

    name NVARCHAR(100) NOT NULL,
    code NVARCHAR(100) NOT NULL,
    description NVARCHAR(255) NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_Permission_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_Permission_DeleteFlag DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    CONSTRAINT PK_Tbl_Permission
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_Permission_Code
        UNIQUE (code)
);
GO


-- ============================================================
-- 5. TBL_USER
-- ============================================================

CREATE TABLE Tbl_User
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    email NVARCHAR(255) NOT NULL,
    name NVARCHAR(150) NOT NULL,

    password_hash NVARCHAR(500) NULL,

    role_id INT NOT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_User_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_User_DeleteFlag DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    CONSTRAINT PK_Tbl_User
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_User_Email
        UNIQUE (email),

    CONSTRAINT FK_Tbl_User_Role
        FOREIGN KEY (role_id)
        REFERENCES Tbl_Role(id)
);
GO


-- ============================================================
-- 6. TBL_ROLE_PERMISSION
-- ============================================================

CREATE TABLE Tbl_Role_Permission
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    role_id INT NOT NULL,
    permission_id INT NOT NULL,

    assigned_at DATETIME2 NOT NULL,
    assigned_by BIGINT NULL,

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_RolePermission_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_Role_Permission
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_RolePermission
        UNIQUE (role_id, permission_id),

    CONSTRAINT FK_Tbl_RolePermission_Role
        FOREIGN KEY (role_id)
        REFERENCES Tbl_Role(id),

    CONSTRAINT FK_Tbl_RolePermission_Permission
        FOREIGN KEY (permission_id)
        REFERENCES Tbl_Permission(id),

    CONSTRAINT FK_Tbl_RolePermission_AssignedBy
        FOREIGN KEY (assigned_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 7. TBL_EVENT
-- ============================================================

CREATE TABLE Tbl_Event
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    name NVARCHAR(200) NOT NULL,
    code NVARCHAR(100) NOT NULL,
    description NVARCHAR(1000) NULL,

    event_start_at DATETIME2 NOT NULL,
    event_end_at DATETIME2 NOT NULL,

    voting_start_at DATETIME2 NOT NULL,
    voting_end_at DATETIME2 NOT NULL,

    result_show_start_at DATETIME2 NULL,
    result_show_end_at DATETIME2 NULL,

    status NVARCHAR(30) NOT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_Event_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_Event_DeleteFlag DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    CONSTRAINT PK_Tbl_Event
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_Event_Code
        UNIQUE (code),

    CONSTRAINT CK_Tbl_Event_Status
        CHECK
        (
            status IN
            (
                N'Upcoming',
                N'Active',
                N'Ended',
                N'Published'
            )
        ),

    CONSTRAINT CK_Tbl_Event_Date
        CHECK (event_end_at > event_start_at),

    CONSTRAINT CK_Tbl_Event_VotingDate
        CHECK (voting_end_at > voting_start_at),

    CONSTRAINT CK_Tbl_Event_VotingInsideEvent
        CHECK
        (
            voting_start_at >= event_start_at
            AND
            voting_end_at <= event_end_at
        ),

    CONSTRAINT CK_Tbl_Event_ResultShowDate
        CHECK
        (
            result_show_start_at IS NULL
            OR result_show_end_at IS NULL
            OR result_show_end_at > result_show_start_at
        )
);
GO


-- ============================================================
-- 8. TBL_ADMIN_EVENT
-- ============================================================

CREATE TABLE Tbl_Admin_Event
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    admin_user_id BIGINT NOT NULL,
    event_id BIGINT NOT NULL,

    assigned_at DATETIME2 NOT NULL,
    assigned_by BIGINT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_AdminEvent_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_AdminEvent_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_Admin_Event
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_AdminEvent
        UNIQUE (admin_user_id, event_id),

    CONSTRAINT FK_Tbl_AdminEvent_User
        FOREIGN KEY (admin_user_id)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_AdminEvent_Event
        FOREIGN KEY (event_id)
        REFERENCES Tbl_Event(id),

    CONSTRAINT FK_Tbl_AdminEvent_AssignedBy
        FOREIGN KEY (assigned_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 9. TBL_USER_EVENT
-- ============================================================

CREATE TABLE Tbl_User_Event
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    user_id BIGINT NOT NULL,
    event_id BIGINT NOT NULL,

    event_password_hash NVARCHAR(500) NOT NULL,

    assigned_at DATETIME2 NOT NULL,
    assigned_by BIGINT NULL,

    password_generated_at DATETIME2 NOT NULL,
    password_expires_at DATETIME2 NOT NULL,

    access_status NVARCHAR(20) NOT NULL,

    removed_at DATETIME2 NULL,
    removed_by BIGINT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_UserEvent_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_UserEvent_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_User_Event
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_UserEvent
        UNIQUE (user_id, event_id),

    CONSTRAINT CK_Tbl_UserEvent_AccessStatus
        CHECK
        (
            access_status IN
            (
                N'valid',
                N'expired'
            )
        ),

    CONSTRAINT FK_Tbl_UserEvent_User
        FOREIGN KEY (user_id)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_UserEvent_Event
        FOREIGN KEY (event_id)
        REFERENCES Tbl_Event(id),

    CONSTRAINT FK_Tbl_UserEvent_AssignedBy
        FOREIGN KEY (assigned_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_UserEvent_RemovedBy
        FOREIGN KEY (removed_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 10. TBL_PROJECT_TYPE
-- ============================================================

CREATE TABLE Tbl_Project_Type
(
    id INT IDENTITY(1,1) NOT NULL,

    name NVARCHAR(50) NOT NULL,
    code NVARCHAR(50) NOT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_ProjectType_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_ProjectType_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_Project_Type
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_ProjectType_Name
        UNIQUE (name),

    CONSTRAINT UQ_Tbl_ProjectType_Code
        UNIQUE (code)
);
GO


-- ============================================================
-- 11. TBL_PROJECT
-- ============================================================

CREATE TABLE Tbl_Project
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    event_id BIGINT NOT NULL,
    project_type_id INT NOT NULL,

    name NVARCHAR(200) NOT NULL,
    description NVARCHAR(1000) NULL,

    vote_counts INT NOT NULL
        CONSTRAINT DF_Tbl_Project_VoteCounts DEFAULT (0),

    is_active BIT NOT NULL
        CONSTRAINT DF_Tbl_Project_IsActive DEFAULT (1),

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_Project_DeleteFlag DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    CONSTRAINT PK_Tbl_Project
        PRIMARY KEY (id),

    CONSTRAINT CK_Tbl_Project_VoteCounts
        CHECK (vote_counts >= 0),

    CONSTRAINT FK_Tbl_Project_Event
        FOREIGN KEY (event_id)
        REFERENCES Tbl_Event(id),

    CONSTRAINT FK_Tbl_Project_Type
        FOREIGN KEY (project_type_id)
        REFERENCES Tbl_Project_Type(id),

    CONSTRAINT FK_Tbl_Project_CreatedBy
        FOREIGN KEY (created_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_Project_ModifiedBy
        FOREIGN KEY (modified_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 12. TBL_VOTING_LIMIT
-- ============================================================

CREATE TABLE Tbl_Voting_Limit
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    user_event_id BIGINT NOT NULL,
    project_type_id INT NOT NULL,

    vote_limit INT NOT NULL,
    vote_used INT NOT NULL
        CONSTRAINT DF_Tbl_VotingLimit_VoteUsed DEFAULT (0),

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_VotingLimit_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_Voting_Limit
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_VotingLimit
        UNIQUE (user_event_id, project_type_id),

    CONSTRAINT CK_Tbl_VotingLimit_Limit
        CHECK (vote_limit >= 0),

    CONSTRAINT CK_Tbl_VotingLimit_Used
        CHECK (vote_used >= 0),

    CONSTRAINT CK_Tbl_VotingLimit_UsedNotGreater
        CHECK (vote_used <= vote_limit),

    CONSTRAINT FK_Tbl_VotingLimit_UserEvent
        FOREIGN KEY (user_event_id)
        REFERENCES Tbl_User_Event(id),

    CONSTRAINT FK_Tbl_VotingLimit_ProjectType
        FOREIGN KEY (project_type_id)
        REFERENCES Tbl_Project_Type(id),

    CONSTRAINT FK_Tbl_VotingLimit_CreatedBy
        FOREIGN KEY (created_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_VotingLimit_ModifiedBy
        FOREIGN KEY (modified_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 13. TBL_VOTE
-- ============================================================

CREATE TABLE Tbl_Vote
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    user_event_id BIGINT NOT NULL,
    project_id BIGINT NOT NULL,

    voted_at DATETIME2 NOT NULL,

    CONSTRAINT PK_Tbl_Vote
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_Vote_UserEvent_Project
        UNIQUE (user_event_id, project_id),

    CONSTRAINT FK_Tbl_Vote_UserEvent
        FOREIGN KEY (user_event_id)
        REFERENCES Tbl_User_Event(id),

    CONSTRAINT FK_Tbl_Vote_Project
        FOREIGN KEY (project_id)
        REFERENCES Tbl_Project(id)
);
GO


-- ============================================================
-- 14. TBL_RESULT
-- ============================================================

CREATE TABLE Tbl_Result
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    event_id BIGINT NOT NULL,
    project_id BIGINT NOT NULL,

    status NVARCHAR(30) NOT NULL,

    version INT NOT NULL,

    reviewed_at DATETIME2 NULL,
    reviewed_by BIGINT NULL,

    published_at DATETIME2 NULL,
    published_by BIGINT NULL,

    created_at DATETIME2 NOT NULL,
    created_by BIGINT NULL,

    modified_at DATETIME2 NULL,
    modified_by BIGINT NULL,

    delete_flag BIT NOT NULL
        CONSTRAINT DF_Tbl_Result_DeleteFlag DEFAULT (0),

    CONSTRAINT PK_Tbl_Result
        PRIMARY KEY (id),

    CONSTRAINT UQ_Tbl_Result
        UNIQUE (event_id, project_id, version),

    CONSTRAINT CK_Tbl_Result_Status
        CHECK
        (
            status IN
            (
                N'first_prize',
                N'second_prize',
                N'third_prize',
                N'normal'
            )
        ),

    CONSTRAINT CK_Tbl_Result_Version
        CHECK (version > 0),

    CONSTRAINT FK_Tbl_Result_Event
        FOREIGN KEY (event_id)
        REFERENCES Tbl_Event(id),

    CONSTRAINT FK_Tbl_Result_Project
        FOREIGN KEY (project_id)
        REFERENCES Tbl_Project(id),

    CONSTRAINT FK_Tbl_Result_ReviewedBy
        FOREIGN KEY (reviewed_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_Result_PublishedBy
        FOREIGN KEY (published_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_Result_CreatedBy
        FOREIGN KEY (created_by)
        REFERENCES Tbl_User(id),

    CONSTRAINT FK_Tbl_Result_ModifiedBy
        FOREIGN KEY (modified_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 15. TBL_AUDIT_LOG
-- ============================================================

CREATE TABLE Tbl_Audit_Log
(
    id BIGINT IDENTITY(1,1) NOT NULL,

    action NVARCHAR(100) NOT NULL,
    entity_type NVARCHAR(100) NOT NULL,
    entity_id BIGINT NULL,

    performed_by BIGINT NULL,
    performed_at DATETIME2 NOT NULL,

    old_value NVARCHAR(MAX) NULL,
    new_value NVARCHAR(MAX) NULL,

    description NVARCHAR(1000) NULL,

    CONSTRAINT PK_Tbl_Audit_Log
        PRIMARY KEY (id),

    CONSTRAINT FK_Tbl_AuditLog_PerformedBy
        FOREIGN KEY (performed_by)
        REFERENCES Tbl_User(id)
);
GO


-- ============================================================
-- 16. SEED ROLES
-- ============================================================

INSERT INTO Tbl_Role
(
    name,
    description,
    created_at
)
VALUES
(
    N'SuperAdmin',
    N'Full system access',
    SYSUTCDATETIME()
),
(
    N'Admin',
    N'Event management access',
    SYSUTCDATETIME()
),
(
    N'User',
    N'Voting access',
    SYSUTCDATETIME()
);
GO


-- ============================================================
-- 17. SEED PROJECT TYPES
-- ============================================================

INSERT INTO Tbl_Project_Type
(
    name,
    code
)
VALUES
(
    N'Hardware',
    N'HARDWARE'
),
(
    N'Software',
    N'SOFTWARE'
);
GO


-- ============================================================
-- 18. CREATE INDEXES
-- ============================================================

CREATE INDEX IX_Tbl_User_RoleId
ON Tbl_User(role_id);
GO

CREATE INDEX IX_Tbl_Admin_Event_AdminUserId
ON Tbl_Admin_Event(admin_user_id);
GO

CREATE INDEX IX_Tbl_Admin_Event_EventId
ON Tbl_Admin_Event(event_id);
GO

CREATE INDEX IX_Tbl_User_Event_UserId
ON Tbl_User_Event(user_id);
GO

CREATE INDEX IX_Tbl_User_Event_EventId
ON Tbl_User_Event(event_id);
GO

CREATE INDEX IX_Tbl_Project_EventId
ON Tbl_Project(event_id);
GO

CREATE INDEX IX_Tbl_Project_ProjectTypeId
ON Tbl_Project(project_type_id);
GO

CREATE INDEX IX_Tbl_Voting_Limit_UserEventId
ON Tbl_Voting_Limit(user_event_id);
GO

CREATE INDEX IX_Tbl_Vote_UserEventId
ON Tbl_Vote(user_event_id);
GO

CREATE INDEX IX_Tbl_Vote_ProjectId
ON Tbl_Vote(project_id);
GO

CREATE INDEX IX_Tbl_Result_EventId
ON Tbl_Result(event_id);
GO

CREATE INDEX IX_Tbl_Result_ProjectId
ON Tbl_Result(project_id);
GO

CREATE INDEX IX_Tbl_Audit_Log_PerformedBy
ON Tbl_Audit_Log(performed_by);
GO

CREATE INDEX IX_Tbl_Audit_Log_Entity
ON Tbl_Audit_Log(entity_type, entity_id);
GO


-- ============================================================
-- 19. VERIFY TABLES
-- ============================================================

SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO