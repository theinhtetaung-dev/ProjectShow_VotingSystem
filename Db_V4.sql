/* ============================================================
   VOTING SYSTEM DATABASE - VERSION 4
   Adds email account management and persisted Round Robin state
   without resetting existing data.
   Target database: VotingSystemDb
   ============================================================ */

USE VotingSystemDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Tbl_Email_Account', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tbl_Email_Account
    (
        id BIGINT IDENTITY(1,1) NOT NULL,
        email_address NVARCHAR(255) NOT NULL,
        display_name NVARCHAR(150) NULL,
        encrypted_app_password NVARCHAR(2000) NOT NULL,
        priority_order INT NOT NULL,

        is_active BIT NOT NULL
            CONSTRAINT DF_Tbl_Email_Account_IsActive DEFAULT (1),

        total_emails_sent INT NOT NULL
            CONSTRAINT DF_Tbl_Email_Account_TotalEmailsSent DEFAULT (0),

        last_used_at DATETIME2 NULL,

        delete_flag BIT NOT NULL
            CONSTRAINT DF_Tbl_Email_Account_DeleteFlag DEFAULT (0),

        created_at DATETIME2 NOT NULL,
        created_by BIGINT NULL,

        modified_at DATETIME2 NULL,
        modified_by BIGINT NULL,

        CONSTRAINT PK_Tbl_Email_Account
            PRIMARY KEY (id),

        CONSTRAINT CK_Tbl_Email_Account_PriorityOrder
            CHECK (priority_order > 0),

        CONSTRAINT CK_Tbl_Email_Account_TotalEmailsSent
            CHECK (total_emails_sent >= 0),

        CONSTRAINT FK_Tbl_EmailAccount_CreatedBy
            FOREIGN KEY (created_by)
            REFERENCES dbo.Tbl_User(id),

        CONSTRAINT FK_Tbl_EmailAccount_ModifiedBy
            FOREIGN KEY (modified_by)
            REFERENCES dbo.Tbl_User(id)
    );
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UQ_Tbl_Email_Account_EmailAddress'
      AND object_id = OBJECT_ID(N'dbo.Tbl_Email_Account'))
BEGIN
    CREATE UNIQUE INDEX UQ_Tbl_Email_Account_EmailAddress
        ON dbo.Tbl_Email_Account(email_address)
        WHERE delete_flag = 0;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_Tbl_Email_Account_ActiveSelection'
      AND object_id = OBJECT_ID(N'dbo.Tbl_Email_Account'))
BEGIN
    CREATE INDEX IX_Tbl_Email_Account_ActiveSelection
        ON dbo.Tbl_Email_Account(is_active, delete_flag, priority_order, id);
END;
GO

IF OBJECT_ID(N'dbo.Tbl_Email_Round_Robin_State', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tbl_Email_Round_Robin_State
    (
        id INT NOT NULL,
        last_selected_account_id BIGINT NULL,
        last_selected_order INT NULL,
        updated_at DATETIME2 NOT NULL,

        CONSTRAINT PK_Tbl_Email_Round_Robin_State
            PRIMARY KEY (id),

        CONSTRAINT CK_Tbl_Email_Round_Robin_State_Singleton
            CHECK (id = 1),

        CONSTRAINT FK_Tbl_EmailRoundRobinState_Account
            FOREIGN KEY (last_selected_account_id)
            REFERENCES dbo.Tbl_Email_Account(id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Tbl_Email_Round_Robin_State WHERE id = 1)
BEGIN
    INSERT INTO dbo.Tbl_Email_Round_Robin_State
    (
        id,
        last_selected_account_id,
        last_selected_order,
        updated_at
    )
    VALUES
    (
        1,
        NULL,
        NULL,
        SYSUTCDATETIME()
    );
END;
GO
