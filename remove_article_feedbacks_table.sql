-- Remove ArticleFeedbacks table from database
-- Run this script in SQL Server Management Studio on NexDeskDb database

USE [NexDeskDb]
GO

-- Drop the ArticleFeedbacks table if it exists
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArticleFeedbacks]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[ArticleFeedbacks];
    PRINT 'ArticleFeedbacks table dropped successfully';
END
ELSE
BEGIN
    PRINT 'ArticleFeedbacks table does not exist';
END
GO

