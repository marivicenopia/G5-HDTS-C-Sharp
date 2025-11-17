-- Add Departments table to NexDeskDb database
USE [NexDeskDb]

-- Create Departments table if it doesn't exist
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

PRINT 'Departments table created successfully' END ELSE BEGIN PRINT 'Departments table already exists' END

-- Insert initial departments if table is empty
IF NOT EXISTS (SELECT * FROM [dbo].[Departments])
BEGIN
    INSERT INTO [dbo].[Departments] ([Id], [Name], [Description], [IsActive], [CreatedTime], [CreatedBy], [UpdatedTime], [UpdatedBy])
    VALUES 
        ('1', 'IT', 'Information Technology Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('2', 'HR', 'Human Resources Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('3', 'Finance', 'Finance Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('4', 'Operations', 'Operations Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('5', 'Marketing', 'Marketing Department', 1, GETDATE(), 'System', GETDATE(), 'System'),
        ('6', 'Customer Support', 'Customer Support Department', 1, GETDATE(), 'System', GETDATE(), 'System')

PRINT 'Initial departments data inserted successfully' END ELSE BEGIN PRINT 'Departments table already contains data' END