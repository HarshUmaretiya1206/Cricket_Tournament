-- ========================================
-- Quick Fix for Existing Database
-- ========================================
USE CricketDB;
GO

-- Drop and recreate the spRegisterUser procedure to accept Role parameter
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spRegisterUser')
    DROP PROCEDURE spRegisterUser;
GO

CREATE PROCEDURE spRegisterUser
    @UserName NVARCHAR(100),
    @Email NVARCHAR(256),
    @PasswordHash NVARCHAR(256),
    @Role NVARCHAR(50) = 'Viewer'
AS
BEGIN
    INSERT INTO Users (UserName, Email, PasswordHash, Role)
    VALUES (@UserName, @Email, @PasswordHash, @Role);
END
GO

-- Update spLoginUser to return Role
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'spLoginUser')
    DROP PROCEDURE spLoginUser;
GO

CREATE PROCEDURE spLoginUser
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT Id, UserName, Email, Role
    FROM Users
    WHERE UserName = @UserName AND PasswordHash = @PasswordHash;
END
GO

-- Add Role column to Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'Role')
BEGIN
    ALTER TABLE Users ADD Role NVARCHAR(50) NOT NULL DEFAULT 'Viewer';
END
GO

-- Update existing users to have a default role
UPDATE Users SET Role = 'Viewer' WHERE Role IS NULL OR Role = '';
GO

-- Insert admin user if it doesn't exist
IF NOT EXISTS (SELECT * FROM Users WHERE UserName = 'admin')
BEGIN
    INSERT INTO Users (UserName, Email, PasswordHash, Role) 
    VALUES ('admin', 'admin@cricheroes.com', 'admin123', 'Admin');
END
GO

PRINT 'Stored procedures updated successfully!';
PRINT 'Users table now supports roles.';
PRINT 'Default admin user: admin/admin123';
GO
