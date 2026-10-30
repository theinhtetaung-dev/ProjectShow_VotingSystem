/* ============================================================
   VOTING SYSTEM DATABASE - VERSION 3
   Adds refresh-token sessions without resetting existing data.
   Target database: VotingSystemDb
   ============================================================ */

USE VotingSystemDb;
GO

IF OBJECT_ID(N'Tbl_Refresh_Token', N'U') IS NULL
BEGIN
    CREATE TABLE Tbl_Refresh_Token
    (
        id BIGINT IDENTITY(1,1) NOT NULL,
        user_id BIGINT NOT NULL,
        token_hash CHAR(64) NOT NULL,
        token_family_id UNIQUEIDENTIFIER NOT NULL,
        expires_at DATETIME2 NOT NULL,
        created_at DATETIME2 NOT NULL,
        last_used_at DATETIME2 NULL,
        revoked_at DATETIME2 NULL,
        replaced_by_token_id BIGINT NULL,
        created_by_ip NVARCHAR(45) NULL,
        revoked_by_ip NVARCHAR(45) NULL,
        device_name NVARCHAR(200) NULL,
        revocation_reason NVARCHAR(255) NULL,

        CONSTRAINT PK_Tbl_Refresh_Token
            PRIMARY KEY (id),

        CONSTRAINT UQ_Tbl_Refresh_Token_Hash
            UNIQUE (token_hash),

        CONSTRAINT FK_Tbl_RefreshToken_User
            FOREIGN KEY (user_id)
            REFERENCES Tbl_User(id),

        CONSTRAINT FK_Tbl_RefreshToken_ReplacedBy
            FOREIGN KEY (replaced_by_token_id)
            REFERENCES Tbl_Refresh_Token(id)
    );

    CREATE INDEX IX_Tbl_Refresh_Token_UserId
        ON Tbl_Refresh_Token(user_id);

    CREATE INDEX IX_Tbl_Refresh_Token_FamilyId
        ON Tbl_Refresh_Token(token_family_id);

    CREATE INDEX IX_Tbl_Refresh_Token_ActiveLookup
        ON Tbl_Refresh_Token(user_id, expires_at, revoked_at);
END;
GO
