IF COL_LENGTH(N'dbo.AppUser', N'AuthVersion') IS NULL
BEGIN
    ALTER TABLE dbo.AppUser
        ADD AuthVersion INT NOT NULL
            CONSTRAINT DF_AppUser_AuthVersion DEFAULT (1) WITH VALUES;
END;
GO

-- =============================================
-- Description: Get a user and role by username for authentication.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_GetByUsername
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.UserId,
        u.Username,
        u.FullName,
        u.Email,
        u.PasswordHash,
        u.PasswordSalt,
        u.PasswordIterations,
        u.AuthVersion,
        u.IsActive,
        u.RoleId,
        r.RoleName
    FROM
        dbo.AppUser u
    INNER JOIN
        dbo.Role r ON u.RoleId = r.RoleId
    WHERE
        u.Username = @Username;
END;
GO

-- =============================================
-- Description: Search users by prefix and status, returning total count and a page of results.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_Search
    @Username VARCHAR(50) = '',
    @FullName NVARCHAR(150) = N'',
    @IsActive BIT = NULL,
    @Offset BIGINT = 0,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsernameFilter VARCHAR(50) = ISNULL(@Username, '');
    DECLARE @FullNameFilter NVARCHAR(150) = ISNULL(@FullName, N'');

    SELECT
        COUNT(*) AS TotalCount
    FROM
        dbo.AppUser
    WHERE
        Username LIKE @UsernameFilter + '%'
        AND FullName LIKE @FullNameFilter + N'%'
        AND (@IsActive IS NULL OR IsActive = @IsActive);

    SELECT
        UserId,
        Username,
        FullName,
        Email,
        IsActive
    FROM
        dbo.AppUser
    WHERE
        Username LIKE @UsernameFilter + '%'
        AND FullName LIKE @FullNameFilter + N'%'
        AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY
        UserId
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- =============================================
-- Description: Get a user profile by user ID.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_GetById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UserId,
        Username,
        FullName,
        Email,
        IsActive
    FROM
        dbo.AppUser
    WHERE
        UserId = @UserId;
END;
GO

-- =============================================
-- Description: Update an existing user profile and return the affected row count.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_Update
    @UserId INT,
    @FullName NVARCHAR(150),
    @Email VARCHAR(254),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        dbo.AppUser
    SET
        FullName = @FullName,
        Email = @Email,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME()
    WHERE
        UserId = @UserId;

    SELECT
        @@ROWCOUNT AS RowsAffected;
END;
GO

-- =============================================
-- Description: Create an active user with the default role.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_Create
    @Username VARCHAR(50),
    @FullName NVARCHAR(150),
    @Email VARCHAR(254),
    @PasswordHash VARBINARY(32),
    @PasswordSalt VARBINARY(16),
    @PasswordIterations INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IsActive BIT = 1;
    DECLARE @RoleId INT;

    SELECT
        @RoleId = RoleId
    FROM
        dbo.Role
    WHERE
        RoleName = 'User';

    INSERT INTO
        dbo.AppUser
    (
        Username,
        FullName,
        Email,
        PasswordHash,
        PasswordSalt,
        PasswordIterations,
        IsActive,
        RoleId
    )
    VALUES
    (
        @Username,
        @FullName,
        @Email,
        @PasswordHash,
        @PasswordSalt,
        @PasswordIterations,
        @IsActive,
        @RoleId
    );
END;
GO

-- =============================================
-- Description: Change a user's password, invalidate existing tickets and return the affected row count.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_ChangePassword
    @UserId INT,
    @NewHash VARBINARY(32),
    @NewSalt VARBINARY(16),
    @PasswordIterations INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        dbo.AppUser
    SET
        PasswordHash = @NewHash,
        PasswordSalt = @NewSalt,
        PasswordIterations = @PasswordIterations,
        AuthVersion = AuthVersion + 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE
        UserId = @UserId;

    SELECT
        @@ROWCOUNT AS RowsAffected;
END;
GO

-- =============================================
-- Description: Invalidate all existing login tickets for a user on logout.
-- Author:      Tran Hoang Vu
-- Created:     2026/11/12
-- =============================================
CREATE PROCEDURE dbo.usp_AppUser_IncreaseAuthVersion
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE
        dbo.AppUser
    SET
        AuthVersion = AuthVersion + 1,
        UpdatedAt = SYSUTCDATETIME()
    WHERE
        UserId = @UserId;

    SELECT
        @@ROWCOUNT AS RowsAffected;
END;
GO
