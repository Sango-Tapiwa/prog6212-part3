-- Contract Monthly Claim System (CMCS)
-- Database Seed Data Script
-- Description: Inserts initial demo data into CMCS database
-- Database: Microsoft SQL Server

USE CMCSDB;
GO

-- Inserting Demo Users
-- Role Values: 0=Lecturer, 1=Coordinator, 2=Manager, 3=HR

PRINT 'Inserting demo users...';
GO

-- Checks if users already exist, if not insert them
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'lecturer@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('lecturer@university.edu', 'lecturer123', 0, 'John', 'Smith', 350.00, 1);
    PRINT 'Lecturer user created: John Smith';
END
ELSE
BEGIN
    PRINT 'Lecturer user already exists: lecturer@university.edu';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'coordinator@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('coordinator@university.edu', 'coordinator123', 1, 'Jane', 'Doe', 0, 1);
    PRINT 'Coordinator user created: Jane Doe';
END
ELSE
BEGIN
    PRINT 'Coordinator user already exists: coordinator@university.edu';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'manager@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('manager@university.edu', 'manager123', 2, 'Bob', 'Johnson', 0, 1);
    PRINT 'Manager user created: Bob Johnson';
END
ELSE
BEGIN
    PRINT 'Manager user already exists: manager@university.edu';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'hr@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('hr@university.edu', 'hr123', 3, 'Sarah', 'Williams', 0, 1);
    PRINT 'HR user created: Sarah Williams';
END
ELSE
BEGIN
    PRINT 'HR user already exists: hr@university.edu';
END
GO

-- Inserting Additional Lecturer Users for Testing
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'lecturer2@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('lecturer2@university.edu', 'lecturer123', 0, 'Emily', 'Brown', 375.00, 1);
    PRINT 'Additional lecturer user created: Emily Brown';
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'lecturer3@university.edu')
BEGIN
    INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, HourlyRate, IsActive)
    VALUES ('lecturer3@university.edu', 'lecturer123', 0, 'Michael', 'Davis', 400.00, 1);
    PRINT 'Additional lecturer user created: Michael Davis';
END
GO


-- Inserting Sample Claims
-- Status Values: 0=Submitted, 1=UnderReview, 2=Approved, 3=Rejected

PRINT 'Inserting sample claims...';
GO

DECLARE @LecturerId INT = (SELECT UserId FROM Users WHERE Email = 'lecturer@university.edu');
DECLARE @Lecturer2Id INT = (SELECT UserId FROM Users WHERE Email = 'lecturer2@university.edu');
DECLARE @ClaimId INT;

-- Sample Claim 1 - Approved
IF NOT EXISTS (SELECT 1 FROM Claims WHERE UserId = @LecturerId)
BEGIN
    INSERT INTO Claims (SubmissionDate, Status, TotalAmount, ApprovedDate, ApprovalComments, UserId)
    VALUES (DATEADD(day, -30, GETDATE()), 2, 5250.00, DATEADD(day, -25, GETDATE()), 'Approved - all documentation verified', @LecturerId);

    SET @ClaimId = SCOPE_IDENTITY();

    INSERT INTO ClaimItems (Date, HoursWorked, HourlyRate, ActivityDescription, ClaimId)
    VALUES
        (DATEADD(day, -30, GETDATE()), 8.0, 350.00, 'Lecture - Introduction to Programming', @ClaimId),
        (DATEADD(day, -29, GETDATE()), 6.0, 350.00, 'Tutorial - Database Design', @ClaimId),
        (DATEADD(day, -28, GETDATE()), 1.0, 350.00, 'Student Consultation', @ClaimId);

    UPDATE Claims SET TotalAmount = (SELECT SUM(HoursWorked * HourlyRate) FROM ClaimItems WHERE ClaimId = @ClaimId) WHERE ClaimId = @ClaimId;

    PRINT 'Sample approved claim created for John Smith';
END
GO

-- Sample Claim 2 - Submitted
DECLARE @Lecturer2Id INT = (SELECT UserId FROM Users WHERE Email = 'lecturer2@university.edu');
DECLARE @ClaimId2 INT;

IF NOT EXISTS (SELECT 1 FROM Claims WHERE UserId = @Lecturer2Id)
BEGIN
    INSERT INTO Claims (SubmissionDate, Status, TotalAmount, UserId)
    VALUES (DATEADD(day, -5, GETDATE()), 0, 3750.00, @Lecturer2Id);

    SET @ClaimId2 = SCOPE_IDENTITY();

    INSERT INTO ClaimItems (Date, HoursWorked, HourlyRate, ActivityDescription, ClaimId)
    VALUES
        (DATEADD(day, -7, GETDATE()), 5.0, 375.00, 'Lecture - Advanced Web Development', @ClaimId2),
        (DATEADD(day, -6, GETDATE()), 5.0, 375.00, 'Lab Session - React Development', @ClaimId2);

    UPDATE Claims SET TotalAmount = (SELECT SUM(HoursWorked * HourlyRate) FROM ClaimItems WHERE ClaimId = @ClaimId2) WHERE ClaimId = @ClaimId2;

    PRINT 'Sample submitted claim created for Emily Brown';
END
GO

-- Sample Claim 3 - Under Review
DECLARE @LecturerId INT = (SELECT UserId FROM Users WHERE Email = 'lecturer@university.edu');
DECLARE @ClaimId3 INT;

INSERT INTO Claims (SubmissionDate, Status, TotalAmount, UserId)
VALUES (DATEADD(day, -3, GETDATE()), 1, 2800.00, @LecturerId);

SET @ClaimId3 = SCOPE_IDENTITY();

INSERT INTO ClaimItems (Date, HoursWorked, HourlyRate, ActivityDescription, ClaimId)
VALUES
    (DATEADD(day, -4, GETDATE()), 4.0, 350.00, 'Lecture - Software Engineering', @ClaimId3),
    (DATEADD(day, -3, GETDATE()), 4.0, 350.00, 'Tutorial - Agile Methodologies', @ClaimId3);

UPDATE Claims SET TotalAmount = (SELECT SUM(HoursWorked * HourlyRate) FROM ClaimItems WHERE ClaimId = @ClaimId3) WHERE ClaimId = @ClaimId3;

PRINT 'Sample under review claim created for John Smith';
GO

-- Displaying Summary
PRINT '================================================';
PRINT 'Data insertion completed successfully!';
PRINT '================================================';
PRINT '';
PRINT 'Summary:';
SELECT
    'Total Users' AS Category,
    COUNT(*) AS Count
FROM Users
UNION ALL
SELECT
    'Total Claims',
    COUNT(*)
FROM Claims
UNION ALL
SELECT
    'Total Claim Items',
    COUNT(*)
FROM ClaimItems;
GO

PRINT '';
PRINT 'User Accounts Created:';
SELECT
    CONCAT(FirstName, ' ', LastName) AS Name,
    Email,
    CASE Role
        WHEN 0 THEN 'Lecturer'
        WHEN 1 THEN 'Coordinator'
        WHEN 2 THEN 'Manager'
        WHEN 3 THEN 'HR'
    END AS Role,
    HourlyRate,
    CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS Status
FROM Users
ORDER BY Role, LastName;
GO

PRINT '';
PRINT '================================================';
PRINT 'Login Credentials:';
PRINT 'Lecturer: lecturer@university.edu / lecturer123';
PRINT 'Coordinator: coordinator@university.edu / coordinator123';
PRINT 'Manager: manager@university.edu / manager123';
PRINT 'HR: hr@university.edu / hr123';
PRINT '================================================';
GO
