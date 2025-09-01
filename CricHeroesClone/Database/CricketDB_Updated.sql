-- ========================================
-- Drop old DB if exists (Force close connections)
-- ========================================
USE master;
GO

IF DB_ID('CricketDB') IS NOT NULL
BEGIN
    ALTER DATABASE CricketDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CricketDB;
END
GO

-- ========================================
-- Create new DB
-- ========================================
CREATE DATABASE CricketDB;
GO

USE CricketDB;
GO

-- ========================================
-- Create Tables
-- ========================================

-- Users Table (for Login/Register with Roles)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserName NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'Viewer', -- Admin, Scorer, Captain, Viewer
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Tournaments
CREATE TABLE Tournaments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    StartDate DATE NULL,
    EndDate DATE NULL
);
GO

-- Teams
CREATE TABLE Teams (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    TournamentId INT NOT NULL,
    CaptainId INT NULL, -- Reference to Users table for team captain
    FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE,
    FOREIGN KEY (CaptainId) REFERENCES Users(Id)
);
GO

-- Players
CREATE TABLE Players (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NULL, -- Batsman, Bowler, All-rounder, Wicket-keeper
    TeamId INT NOT NULL,
    BattingStyle NVARCHAR(50) NULL, -- Right-handed, Left-handed
    BowlingStyle NVARCHAR(50) NULL, -- Fast, Medium, Spin
    FOREIGN KEY (TeamId) REFERENCES Teams(Id) ON DELETE CASCADE
);
GO

-- Matches
CREATE TABLE Matches (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TournamentId INT NOT NULL,
    TeamAId INT NOT NULL,
    TeamBId INT NOT NULL,
    MatchDate DATETIME NULL,
    Venue NVARCHAR(100) NULL,
    Status NVARCHAR(20) DEFAULT 'Scheduled', -- Scheduled, Live, Completed, Cancelled
    TossWinnerTeamId INT NULL,
    Result NVARCHAR(200) NULL,
    FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id) ON DELETE CASCADE,
    FOREIGN KEY (TeamAId) REFERENCES Teams(Id),
    FOREIGN KEY (TeamBId) REFERENCES Teams(Id),
    FOREIGN KEY (TossWinnerTeamId) REFERENCES Teams(Id)
);
GO

-- Scores (optional live scoring table)
CREATE TABLE Scores (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MatchId INT NOT NULL,
    TeamId INT NOT NULL,
    Runs INT DEFAULT 0,
    Wickets INT DEFAULT 0,
    Overs FLOAT DEFAULT 0,
    FOREIGN KEY (MatchId) REFERENCES Matches(Id) ON DELETE CASCADE,
    FOREIGN KEY (TeamId) REFERENCES Teams(Id)
);
GO

-- ========================================
-- Insert Essential Data Only
-- ========================================

-- Insert default admin user
INSERT INTO Users (UserName, Email, PasswordHash, Role) 
VALUES ('admin', 'admin@cricheroes.com', 'admin123', 'Admin');
GO

-- ========================================
-- Stored Procedures
-- ========================================

-- User SPs (Login/Register with Roles)
CREATE OR ALTER PROCEDURE spRegisterUser
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

CREATE OR ALTER PROCEDURE spLoginUser
    @UserName NVARCHAR(100),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT Id, UserName, Email, Role
    FROM Users
    WHERE UserName = @UserName AND PasswordHash = @PasswordHash;
END
GO

CREATE OR ALTER PROCEDURE spGetUserById
    @Id INT
AS
BEGIN
    SELECT Id, UserName, Email, Role, CreatedAt
    FROM Users
    WHERE Id = @Id;
END
GO

CREATE OR ALTER PROCEDURE spDeleteUser
    @Id INT
AS
BEGIN
    DELETE FROM Users WHERE Id = @Id;
END
GO

-- Tournament SPs
CREATE OR ALTER PROCEDURE spAddTournament
    @Name NVARCHAR(100),
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    INSERT INTO Tournaments (Name, StartDate, EndDate)
    VALUES (@Name, @StartDate, @EndDate);
END
GO

CREATE OR ALTER PROCEDURE spGetTournaments
AS
BEGIN
    SELECT * FROM Tournaments;
END
GO

CREATE OR ALTER PROCEDURE spDeleteTournament
    @Id INT
AS
BEGIN
    DELETE FROM Tournaments WHERE Id = @Id;
END
GO

-- Team SPs
CREATE OR ALTER PROCEDURE spAddTeam
    @Name NVARCHAR(100),
    @TournamentId INT,
    @CaptainId INT = NULL
AS
BEGIN
    INSERT INTO Teams (Name, TournamentId, CaptainId) 
    VALUES (@Name, @TournamentId, @CaptainId);
END
GO

CREATE OR ALTER PROCEDURE spGetTeams
AS
BEGIN
    SELECT t.Id, t.Name, tr.Name AS TournamentName, 
           t.CaptainId, ISNULL(u.UserName, 'No Captain') AS CaptainName
    FROM Teams t
    JOIN Tournaments tr ON t.TournamentId = tr.Id
    LEFT JOIN Users u ON t.CaptainId = u.Id;
END
GO

CREATE OR ALTER PROCEDURE spDeleteTeam
    @Id INT
AS
BEGIN
    DELETE FROM Teams WHERE Id = @Id;
END
GO

-- Player SPs
CREATE OR ALTER PROCEDURE spAddPlayer
    @Name NVARCHAR(100),
    @Role NVARCHAR(50),
    @TeamId INT,
    @BattingStyle NVARCHAR(50) = NULL,
    @BowlingStyle NVARCHAR(50) = NULL
AS
BEGIN
    INSERT INTO Players (Name, Role, TeamId, BattingStyle, BowlingStyle) 
    VALUES (@Name, @Role, @TeamId, @BattingStyle, @BowlingStyle);
END
GO

CREATE OR ALTER PROCEDURE spGetPlayers
AS
BEGIN
    SELECT p.Id, p.Name, p.Role, t.Name AS TeamName, 
           p.BattingStyle, p.BowlingStyle, p.TeamId
    FROM Players p
    JOIN Teams t ON p.TeamId = t.Id;
END
GO

CREATE OR ALTER PROCEDURE spDeletePlayer
    @Id INT
AS
BEGIN
    DELETE FROM Players WHERE Id = @Id;
END
GO

-- Match SPs
CREATE OR ALTER PROCEDURE spAddMatch
    @TournamentId INT,
    @TeamAId INT,
    @TeamBId INT,
    @MatchDate DATETIME,
    @Venue NVARCHAR(100)
AS
BEGIN
    INSERT INTO Matches (TournamentId, TeamAId, TeamBId, MatchDate, Venue)
    VALUES (@TournamentId, @TeamAId, @TeamBId, @MatchDate, @Venue);
END
GO

CREATE OR ALTER PROCEDURE spGetMatches
AS
BEGIN
    SELECT m.Id, tr.Name AS TournamentName,
           ta.Name AS TeamAName, tb.Name AS TeamBName,
           m.MatchDate, m.Venue, m.Status, m.Result
    FROM Matches m
    JOIN Tournaments tr ON m.TournamentId = tr.Id
    JOIN Teams ta ON m.TeamAId = ta.Id
    JOIN Teams tb ON m.TeamBId = tb.Id;
END
GO

CREATE OR ALTER PROCEDURE spDeleteMatch
    @Id INT
AS
BEGIN
    DELETE FROM Matches WHERE Id = @Id;
END
GO

-- Score SPs (for live updates)
CREATE OR ALTER PROCEDURE spUpdateScore
    @MatchId INT,
    @TeamId INT,
    @Runs INT,
    @Wickets INT,
    @Overs FLOAT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Scores WHERE MatchId = @MatchId AND TeamId = @TeamId)
        UPDATE Scores SET Runs=@Runs, Wickets=@Wickets, Overs=@Overs
        WHERE MatchId=@MatchId AND TeamId=@TeamId;
    ELSE
        INSERT INTO Scores (MatchId, TeamId, Runs, Wickets, Overs)
        VALUES (@MatchId, @TeamId, @Runs, @Wickets, @Overs);
END
GO

CREATE OR ALTER PROCEDURE spGetScores
    @MatchId INT
AS
BEGIN
    SELECT s.Id, t.Name AS TeamName, s.Runs, s.Wickets, s.Overs
    FROM Scores s
    JOIN Teams t ON s.TeamId = t.Id
    WHERE s.MatchId = @MatchId;
END
GO

-- ========================================
-- Additional Stored Procedures for Enhanced Functionality
-- ========================================

-- Get live matches
CREATE OR ALTER PROCEDURE spGetLiveMatches
AS
BEGIN
    SELECT m.Id, tr.Name AS TournamentName,
           ta.Name AS TeamAName, tb.Name AS TeamBName,
           m.MatchDate, m.Venue, m.Status, m.Result
    FROM Matches m
    JOIN Tournaments tr ON m.TournamentId = tr.Id
    JOIN Teams ta ON m.TeamAId = ta.Id
    JOIN Teams tb ON m.TeamBId = tb.Id
    WHERE m.Status = 'Live';
END
GO

-- Get upcoming matches
CREATE OR ALTER PROCEDURE spGetUpcomingMatches
AS
BEGIN
    SELECT m.Id, tr.Name AS TournamentName,
           ta.Name AS TeamAName, tb.Name AS TeamBName,
           m.MatchDate, m.Venue, m.Status, m.Result
    FROM Matches m
    JOIN Tournaments tr ON m.TournamentId = tr.Id
    JOIN Teams ta ON m.TeamAId = ta.Id
    JOIN Teams tb ON m.TeamBId = tb.Id
    WHERE m.Status = 'Scheduled' AND m.MatchDate > GETDATE()
    ORDER BY m.MatchDate;
END
GO

-- Get upcoming matches by team
CREATE OR ALTER PROCEDURE spGetUpcomingMatchesByTeam
    @TeamId INT
AS
BEGIN
    SELECT m.Id, tr.Name AS TournamentName,
           ta.Name AS TeamAName, tb.Name AS TeamBName,
           m.MatchDate, m.Venue, m.Status, m.Result
    FROM Matches m
    JOIN Tournaments tr ON m.TournamentId = tr.Id
    JOIN Teams ta ON m.TeamAId = ta.Id
    JOIN Teams tb ON m.TeamBId = tb.Id
    WHERE (m.TeamAId = @TeamId OR m.TeamBId = @TeamId) 
          AND m.Status = 'Scheduled' 
          AND m.MatchDate > GETDATE()
    ORDER BY m.MatchDate;
END
GO

-- Get total counts for dashboard
CREATE OR ALTER PROCEDURE spGetDashboardCounts
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM Users) AS TotalUsers,
        (SELECT COUNT(*) FROM Teams) AS TotalTeams,
        (SELECT COUNT(*) FROM Players) AS TotalPlayers,
        (SELECT COUNT(*) FROM Matches) AS TotalMatches,
        (SELECT COUNT(*) FROM Tournaments) AS TotalTournaments;
END
GO

PRINT 'Database CricketDB created successfully with clean structure!';
PRINT 'Default admin user: admin/admin123';
PRINT 'No dummy data - add your own teams, players, and matches as needed.';
GO
