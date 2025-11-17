-- Simple fix for NexDeskDb database schema
-- Rename Department column to DepartmentId in Users table to match Entity Framework model

USE [NexDeskDb]

PRINT 'Starting database schema fix...'

-- Update existing Department values to use proper Department IDs
PRINT 'Updating Department values to use Department IDs...'

UPDATE [dbo].[Users] 
SET [Department] = CASE 
    WHEN [Department] IN ('IT', 'Information Technology') THEN '1'
    WHEN [Department] IN ('HR', 'Human Resources') THEN '2' 
    WHEN [Department] = 'Finance' THEN '3'
    WHEN [Department] = 'Operations' THEN '4'
    WHEN [Department] = 'Marketing' THEN '5'
    WHEN [Department] IN ('Customer Support', 'Support') THEN '6'
    ELSE '1' -- Default to IT department (ID: 1) if no match
END

PRINT 'Department values updated successfully'

-- Rename the column from Department to DepartmentId
PRINT 'Renaming Department column to DepartmentId...'

EXEC sp_rename 'dbo.Users.Department', 'DepartmentId', 'COLUMN'

PRINT 'Column renamed successfully'

-- Verify the change

PRINT 'Verifying schema change...'

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'DepartmentId' AND TABLE_SCHEMA = 'dbo')
BEGIN
    DECLARE @UserCount INT
    SELECT @UserCount = COUNT(*) FROM [dbo].[Users]
    PRINT 'SUCCESS: Users table now has DepartmentId column with ' + CAST(@UserCount AS VARCHAR(10)) + ' users'
END
ELSE
BEGIN
    PRINT 'ERROR: DepartmentId column was not created properly'
END

PRINT 'Database schema fix completed!'