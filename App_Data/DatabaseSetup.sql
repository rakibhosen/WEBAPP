IF DB_ID(N'APP_WEB') IS NULL
BEGIN
    CREATE DATABASE APP_WEB;
END
GO

USE APP_WEB;
GO

IF OBJECT_ID(N'dbo.AppUsers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppUsers
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppUsers PRIMARY KEY,
        UserName NVARCHAR(100) NOT NULL CONSTRAINT UQ_AppUsers_UserName UNIQUE,
        DisplayName NVARCHAR(150) NOT NULL,
        [Password] NVARCHAR(200) NOT NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_AppUsers_IsActive DEFAULT (1),
        CreatedAtUtc DATETIME2(0) NOT NULL CONSTRAINT DF_AppUsers_CreatedAtUtc DEFAULT (SYSUTCDATETIME())
    );
END
GO

IF OBJECT_ID(N'dbo.AppRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppRoles
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppRoles PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_AppRoles_Name UNIQUE,
        DisplayName NVARCHAR(150) NOT NULL
    );
END
GO

IF OBJECT_ID(N'dbo.AppPermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppPermissions
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AppPermissions PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_AppPermissions_Name UNIQUE,
        Description NVARCHAR(250) NULL
    );
END
GO

IF OBJECT_ID(N'dbo.AppUserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppUserRoles
    (
        UserId INT NOT NULL,
        RoleId INT NOT NULL,
        CONSTRAINT PK_AppUserRoles PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_AppUserRoles_AppUsers FOREIGN KEY (UserId) REFERENCES dbo.AppUsers(Id),
        CONSTRAINT FK_AppUserRoles_AppRoles FOREIGN KEY (RoleId) REFERENCES dbo.AppRoles(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.AppRolePermissions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppRolePermissions
    (
        RoleId INT NOT NULL,
        PermissionId INT NOT NULL,
        CONSTRAINT PK_AppRolePermissions PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_AppRolePermissions_AppRoles FOREIGN KEY (RoleId) REFERENCES dbo.AppRoles(Id),
        CONSTRAINT FK_AppRolePermissions_AppPermissions FOREIGN KEY (PermissionId) REFERENCES dbo.AppPermissions(Id)
    );
END
GO

IF OBJECT_ID(N'dbo.AppMenuItems', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AppMenuItems
    (
        [Key] NVARCHAR(100) NOT NULL CONSTRAINT PK_AppMenuItems PRIMARY KEY,
        [Text] NVARCHAR(150) NOT NULL,
        Controller NVARCHAR(100) NULL,
        [Action] NVARCHAR(100) NULL,
        PermissionName NVARCHAR(100) NULL,
        SortOrder INT NOT NULL,
        Icon NVARCHAR(50) NULL,
        ParentKey NVARCHAR(100) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_AppMenuItems_IsActive DEFAULT (1),
        CONSTRAINT FK_AppMenuItems_AppPermissions FOREIGN KEY (PermissionName) REFERENCES dbo.AppPermissions(Name),
        CONSTRAINT FK_AppMenuItems_Parent FOREIGN KEY (ParentKey) REFERENCES dbo.AppMenuItems([Key])
    );
END
GO

MERGE dbo.AppPermissions AS target
USING (VALUES
    (N'Dashboard.View', N'View dashboard'),
    (N'Privacy.View', N'View privacy page'),
    (N'Reports.View', N'View reports'),
    (N'Users.Manage', N'Manage users and roles')
) AS source (Name, Description)
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET Description = source.Description
WHEN NOT MATCHED THEN
    INSERT (Name, Description) VALUES (source.Name, source.Description);
GO

MERGE dbo.AppRoles AS target
USING (VALUES
    (N'Admin', N'System Administrator'),
    (N'Manager', N'Branch Manager'),
    (N'User', N'Standard Operator')
) AS source (Name, DisplayName)
ON target.Name = source.Name
WHEN MATCHED THEN
    UPDATE SET DisplayName = source.DisplayName
WHEN NOT MATCHED THEN
    INSERT (Name, DisplayName) VALUES (source.Name, source.DisplayName);
GO

MERGE dbo.AppUsers AS target
USING (VALUES
    (N'admin', N'System Admin', N'admin123', CONVERT(bit, 1)),
    (N'manager', N'Manager User', N'manager123', CONVERT(bit, 1)),
    (N'user', N'Standard User', N'user123', CONVERT(bit, 1))
) AS source (UserName, DisplayName, [Password], IsActive)
ON target.UserName = source.UserName
WHEN MATCHED THEN
    UPDATE SET DisplayName = source.DisplayName, [Password] = source.[Password], IsActive = source.IsActive
WHEN NOT MATCHED THEN
    INSERT (UserName, DisplayName, [Password], IsActive)
    VALUES (source.UserName, source.DisplayName, source.[Password], source.IsActive);
GO

MERGE dbo.AppRolePermissions AS target
USING
(
    SELECT r.Id AS RoleId, p.Id AS PermissionId
    FROM dbo.AppRoles r
    CROSS JOIN dbo.AppPermissions p
    WHERE r.Name = N'Admin'

    UNION ALL
    SELECT r.Id, p.Id
    FROM dbo.AppRoles r
    INNER JOIN dbo.AppPermissions p ON p.Name IN (N'Dashboard.View', N'Reports.View')
    WHERE r.Name = N'Manager'

    UNION ALL
    SELECT r.Id, p.Id
    FROM dbo.AppRoles r
    INNER JOIN dbo.AppPermissions p ON p.Name = N'Dashboard.View'
    WHERE r.Name = N'User'
) AS source (RoleId, PermissionId)
ON target.RoleId = source.RoleId AND target.PermissionId = source.PermissionId
WHEN NOT MATCHED THEN
    INSERT (RoleId, PermissionId) VALUES (source.RoleId, source.PermissionId);
GO

MERGE dbo.AppUserRoles AS target
USING
(
    SELECT u.Id AS UserId, r.Id AS RoleId
    FROM dbo.AppUsers u
    INNER JOIN dbo.AppRoles r ON
        (u.UserName = N'admin' AND r.Name = N'Admin') OR
        (u.UserName = N'manager' AND r.Name = N'Manager') OR
        (u.UserName = N'user' AND r.Name = N'User')
) AS source (UserId, RoleId)
ON target.UserId = source.UserId AND target.RoleId = source.RoleId
WHEN NOT MATCHED THEN
    INSERT (UserId, RoleId) VALUES (source.UserId, source.RoleId);
GO

MERGE dbo.AppMenuItems AS target
USING (VALUES
    (N'dashboard', N'Dashboard', N'Home', N'Index', N'Dashboard.View', 10, N'grid', NULL, CONVERT(bit, 1)),
    (N'banking', N'Banking Operations', NULL, NULL, NULL, 20, N'landmark', NULL, CONVERT(bit, 1)),
    (N'banking-reports', N'Reports', N'Reports', N'Index', N'Reports.View', 10, N'chart', N'banking', CONVERT(bit, 1)),
    (N'banking-compliance', N'Compliance', NULL, NULL, NULL, 20, N'shield', N'banking', CONVERT(bit, 1)),
    (N'privacy', N'Privacy', N'Home', N'Privacy', N'Privacy.View', 10, N'shield', N'banking-compliance', CONVERT(bit, 1)),
    (N'administration', N'Administration', NULL, NULL, NULL, 30, N'settings', NULL, CONVERT(bit, 1)),
    (N'user-access', N'User Access', N'Admin', N'Users', N'Users.Manage', 10, N'users', N'administration', CONVERT(bit, 1))
) AS source ([Key], [Text], Controller, [Action], PermissionName, SortOrder, Icon, ParentKey, IsActive)
ON target.[Key] = source.[Key]
WHEN MATCHED THEN
    UPDATE SET
        [Text] = source.[Text],
        Controller = source.Controller,
        [Action] = source.[Action],
        PermissionName = source.PermissionName,
        SortOrder = source.SortOrder,
        Icon = source.Icon,
        ParentKey = source.ParentKey,
        IsActive = source.IsActive
WHEN NOT MATCHED THEN
    INSERT ([Key], [Text], Controller, [Action], PermissionName, SortOrder, Icon, ParentKey, IsActive)
    VALUES (source.[Key], source.[Text], source.Controller, source.[Action], source.PermissionName, source.SortOrder, source.Icon, source.ParentKey, source.IsActive);
GO
