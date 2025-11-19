-- Contract Monthly Claim System (CMCS)
-- Database Schema Creation Script
-- Description: Creates all necessary tables and relationships for CMCS
-- Database: Microsoft SQL Server

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CMCSDB')
BEGIN
    CREATE DATABASE CMCSDB;
    PRINT 'Database CMCSDB created successfully.';
END
ELSE
BEGIN
    PRINT 'Database CMCSDB already exists.';
END
GO

USE CMCSDB;
GO

-- Table: Users
-- Stores all system users including Lecturers, Coordinators, Managers, and HR
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND type = 'U')
BEGIN
    CREATE TABLE Users (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role INT NOT NULL,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        HourlyRate DECIMAL(8,2) NOT NULL DEFAULT 350.00,
        IsActive BIT NOT NULL DEFAULT 1,
        CONSTRAINT CK_Users_Role CHECK (Role IN (0, 1, 2, 3)),
        CONSTRAINT CK_Users_HourlyRate CHECK (HourlyRate >= 0 AND HourlyRate <= 1000.00)
    );
    PRINT 'Table Users created successfully.';
END
GO


-- Table: Claims
-- Description: Stores claim submissions by lecturers
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Claims' AND type = 'U')
BEGIN
    CREATE TABLE Claims (
        ClaimId INT IDENTITY(1,1) PRIMARY KEY,
        SubmissionDate DATETIME2(7) NOT NULL,
        Status INT NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        ApprovedDate DATETIME2(7) NULL,
        ApprovalComments NVARCHAR(MAX) NULL,
        UserId INT NOT NULL,
        CONSTRAINT FK_Claims_Users FOREIGN KEY (UserId)
            REFERENCES Users(UserId) ON DELETE CASCADE,
        CONSTRAINT CK_Claims_Status CHECK (Status IN (0, 1, 2, 3))
    );
    PRINT 'Table Claims created successfully.';
END
GO


-- Table: ClaimItems
-- Stores individual line items for each claim
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ClaimItems' AND type = 'U')
BEGIN
    CREATE TABLE ClaimItems (
        ItemId INT IDENTITY(1,1) PRIMARY KEY,
        Date DATETIME2(7) NOT NULL,
        HoursWorked DECIMAL(5,2) NOT NULL,
        HourlyRate DECIMAL(8,2) NOT NULL,
        ActivityDescription NVARCHAR(500) NOT NULL,
        ClaimId INT NOT NULL,
        CONSTRAINT FK_ClaimItems_Claims FOREIGN KEY (ClaimId)
            REFERENCES Claims(ClaimId) ON DELETE CASCADE,
        CONSTRAINT CK_ClaimItems_HoursWorked CHECK (HoursWorked >= 0.1 AND HoursWorked <= 24.0),
        CONSTRAINT CK_ClaimItems_HourlyRate CHECK (HourlyRate >= 0.01 AND HourlyRate <= 1000.00)
    );
    PRINT 'Table ClaimItems created successfully.';
END
GO


-- Table: Documents
-- Stores supporting documents for claims
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Documents' AND type = 'U')
BEGIN
    CREATE TABLE Documents (
        DocumentId INT IDENTITY(1,1) PRIMARY KEY,
        FileName NVARCHAR(255) NOT NULL,
        FilePath NVARCHAR(500) NOT NULL,
        UploadDate DATETIME2(7) NOT NULL,
        FileSize BIGINT NOT NULL,
        ContentType NVARCHAR(100) NULL,
        ClaimId INT NOT NULL,
        CONSTRAINT FK_Documents_Claims FOREIGN KEY (ClaimId)
            REFERENCES Claims(ClaimId) ON DELETE CASCADE
    );
    PRINT 'Table Documents created successfully.';
END
GO


-- Create Indexes for Performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_UserId' AND object_id = OBJECT_ID('Claims'))
BEGIN
    CREATE INDEX IX_Claims_UserId ON Claims(UserId);
    PRINT 'Index IX_Claims_UserId created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_Status' AND object_id = OBJECT_ID('Claims'))
BEGIN
    CREATE INDEX IX_Claims_Status ON Claims(Status);
    PRINT 'Index IX_Claims_Status created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Claims_SubmissionDate' AND object_id = OBJECT_ID('Claims'))
BEGIN
    CREATE INDEX IX_Claims_SubmissionDate ON Claims(SubmissionDate);
    PRINT 'Index IX_Claims_SubmissionDate created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ClaimItems_ClaimId' AND object_id = OBJECT_ID('ClaimItems'))
BEGIN
    CREATE INDEX IX_ClaimItems_ClaimId ON ClaimItems(ClaimId);
    PRINT 'Index IX_ClaimItems_ClaimId created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Documents_ClaimId' AND object_id = OBJECT_ID('Documents'))
BEGIN
    CREATE INDEX IX_Documents_ClaimId ON Documents(ClaimId);
    PRINT 'Index IX_Documents_ClaimId created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Email ON Users(Email);
    PRINT 'Index IX_Users_Email created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Role' AND object_id = OBJECT_ID('Users'))
BEGIN
    CREATE INDEX IX_Users_Role ON Users(Role);
    PRINT 'Index IX_Users_Role created successfully.';
END
GO

PRINT 'Database schema creation completed successfully!';
GO
