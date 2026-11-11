ALTER   PROCEDURE dbo.usp_AppUser_GetByUsername @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.UserId, u.Username, u.FullName, u.Email, u.PasswordHash,
           u.PasswordSalt, u.PasswordIterations, u.IsActive, u.RoleId, r.RoleName
    FROM dbo.AppUser u
    INNER JOIN dbo.Role r ON u.RoleId = r.RoleId
    WHERE u.Username = @Username;
END;
GO

ALTER   PROCEDURE dbo.usp_AppUser_Search
    @Username VARCHAR(50) = '',
    @FullName NVARCHAR(150) = N'',
    @IsActive BIT = NULL,
    @Offset BIGINT = 0,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS TotalCount
    FROM dbo.AppUser
    WHERE Username LIKE '%' + @Username + '%'
      AND FullName LIKE N'%' + @FullName + N'%'
      AND (@IsActive IS NULL OR IsActive = @IsActive);

    SELECT UserId, Username, FullName, Email, IsActive
    FROM dbo.AppUser
    WHERE Username LIKE '%' + @Username + '%'
      AND FullName LIKE N'%' + @FullName + N'%'
      AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY UserId
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

ALTER   PROCEDURE dbo.usp_AppUser_GetById @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Username, FullName, Email, IsActive
    FROM dbo.AppUser
    WHERE UserId = @UserId;
END;
GO

ALTER   PROCEDURE dbo.usp_AppUser_Update
    @UserId INT,
    @FullName NVARCHAR(150),
    @Email VARCHAR(254),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.AppUser
    SET FullName = @FullName,
        Email = @Email,
        IsActive = @IsActive,
        UpdatedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId;

    SELECT @@ROWCOUNT;
END;
GO

ALTER   PROCEDURE dbo.usp_AppUser_Create
    @Username VARCHAR(50),
    @FullName NVARCHAR(150),
    @Email VARCHAR(254),
    @PasswordHash VARBINARY(32),
    @PasswordSalt VARBINARY(16),
    @PasswordIterations INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.AppUser
        (Username, FullName, Email, PasswordHash, PasswordSalt, PasswordIterations, IsActive, RoleId)
    VALUES
        (@Username, @FullName, @Email, @PasswordHash, @PasswordSalt, @PasswordIterations, 1, 2);
END;
GO

ALTER   PROCEDURE dbo.usp_AppUser_ChangePassword
    @UserId INT,
    @NewHash VARBINARY(32),
    @NewSalt VARBINARY(16),
    @PasswordIterations INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.AppUser
    SET PasswordHash = @NewHash,
        PasswordSalt = @NewSalt,
        PasswordIterations = @PasswordIterations,
        UpdatedAt = SYSUTCDATETIME()
    WHERE UserId = @UserId;

    SELECT @@ROWCOUNT;
END;
GO
