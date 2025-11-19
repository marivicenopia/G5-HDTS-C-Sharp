-- =====================================================
-- Fix NexDeskDb Tickets Table Schema
-- =====================================================
-- This script updates the Tickets table to support GUID IDs
-- The primary key constraint must be dropped first

USE NexDeskDb;
GO

-- =====================================================
-- Step 1: Check current structure
-- =====================================================
PRINT 'Current Tickets table structure:';
EXEC sp_help '[dbo].[Tickets]';
GO

-- =====================================================
-- Step 2: Drop the primary key constraint
-- =====================================================
PRINT 'Dropping primary key constraint PK_ticketsdb...';
ALTER TABLE [dbo].[Tickets]
DROP CONSTRAINT [PK_ticketsdb];
GO

PRINT 'Primary key constraint dropped successfully!';
GO

-- =====================================================
-- Step 3: Alter the Id column to support GUID
-- =====================================================
PRINT 'Altering dbo.Tickets.Id column to NVARCHAR(450)...';
ALTER TABLE [dbo].[Tickets]
ALTER COLUMN [Id] NVARCHAR(450) NOT NULL;
GO

PRINT 'Column altered successfully!';
GO

-- =====================================================
-- Step 4: Recreate the primary key constraint
-- =====================================================
PRINT 'Recreating primary key constraint...';
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [PK_ticketsdb] PRIMARY KEY CLUSTERED ([Id] ASC);
GO

PRINT 'Primary key constraint recreated successfully!';
GO

-- =====================================================
-- Step 5: Verify the changes
-- =====================================================
PRINT 'Verifying table structure after changes:';
EXEC sp_help '[dbo].[Tickets]';
GO

-- =====================================================
-- Step 6: Display column details
-- =====================================================
PRINT 'Ticket table columns:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
 IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Tickets'
  AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;
GO

PRINT '========================================';
PRINT 'Script completed successfully!';
PRINT 'Tickets.Id is now NVARCHAR(450)';
PRINT '========================================';
GO
