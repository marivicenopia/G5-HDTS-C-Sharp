-- Fix NexDeskDb database schema to match Entity Framework models
-- Run this script to resolve column name mismatches and missing tables

USE [NexDeskDb]

-- First, create Departments table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Departments](
        [Id] [nvarchar](450) NOT NULL,
        [Name] [nvarchar](max) NULL,
        [Description] [nvarchar](max) NULL,
        [IsActive] [bit] NOT NULL,
        [CreatedTime] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](max) NULL,
        [UpdatedTime] [datetime2](7) NOT NULL,
        [UpdatedBy] [nvarchar](max) NULL,
     CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

PRINT 'Departments table created successfully'

-- Insert initial departments
INSERT INTO [dbo].[Departments] ([Id], [Name], [Description], [IsActive], [CreatedTime], [CreatedBy], [UpdatedTime], [UpdatedBy])
    VALUES 
        ('1', 'IT', 'Information Technology Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('2', 'HR', 'Human Resources Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('3', 'Finance', 'Finance Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('4', 'Operations', 'Operations Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('5', 'Marketing', 'Marketing Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('6', 'Customer Support', 'Customer Support Department', 1, GETDATE(), 'System', GETDATE(), 'System')

PRINT 'Initial departments data inserted successfully' END ELSE BEGIN PRINT 'Departments table already exists' END

-- Fix Users table - rename Department column to DepartmentId if it exists
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
        TABLE_NAME = 'Users'
        AND COLUMN_NAME = 'Department'
        AND TABLE_SCHEMA = 'dbo'
) BEGIN
-- First, update existing Department values to map to new DepartmentId values
-- This maps string department names to department IDs

PRINT 'Updating Users table Department column values to match Department IDs...'

UPDATE [dbo].[Users] 
    SET [Department] = CASE 
        WHEN [Department] = 'IT' OR [Department] = 'Information Technology' THEN '1'
        WHEN [Department] = 'HR' OR [Department] = 'Human Resources' THEN '2' 
        WHEN [Department] = 'Finance' THEN '3'
        WHEN [Department] = 'Operations' THEN '4'
        WHEN [Department] = 'Marketing' THEN '5'
        WHEN [Department] = 'Customer Support' OR [Department] = 'Support' THEN '6'
        ELSE '1' -- Default to IT department if no match
    END
    WHERE [Department] IS NOT NULL

PRINT 'Department values updated to IDs'

-- Now rename the column
EXEC sp_rename 'dbo.Users.Department', 'DepartmentId', 'COLUMN'

PRINT 'Users table Department column renamed to DepartmentId successfully'
END
ELSE IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'DepartmentId' AND TABLE_SCHEMA = 'dbo')
BEGIN
    -- If neither Department nor DepartmentId exists, add DepartmentId column
    ALTER TABLE [dbo].[Users] ADD [DepartmentId] [varchar](50) NULL

-- Set default department ID for existing users
UPDATE [dbo].[Users] SET [DepartmentId] = '1' WHERE [DepartmentId] IS NULL

PRINT 'DepartmentId column added to Users table' END ELSE BEGIN PRINT 'Users table already has DepartmentId column' END

-- Verify the schema changes
PRINT 'Verifying schema changes...'

-- Check if Departments table exists and has data
IF EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
BEGIN
    DECLARE @DeptCount INT
    SELECT @DeptCount = COUNT(*) FROM [dbo].[Departments]
    PRINT 'Departments table exists with ' + CAST(@DeptCount AS VARCHAR(10)) + ' records'
END

-- Check if Users table has DepartmentId column
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND COLUMN_NAME = 'DepartmentId' AND TABLE_SCHEMA = 'dbo')
BEGIN
    DECLARE @UserCount INT
    SELECT @UserCount = COUNT(*) FROM [dbo].[Users] WHERE [DepartmentId] IS NOT NULL
    PRINT 'Users table has DepartmentId column with ' + CAST(@UserCount AS VARCHAR(10)) + ' users having department assignments'
END

PRINT 'Database schema fix completed successfully!'