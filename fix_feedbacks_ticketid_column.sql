-- Fix TicketId column size in Feedbacks table
-- The current size of varchar(20) is too small for GUID values (36 characters)

USE [NexDeskDb]

-- Check if the Feedbacks table exists and has the TicketId column
IF EXISTS (
    SELECT *
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
        TABLE_NAME = 'Feedbacks'
        AND COLUMN_NAME = 'TicketId'
) BEGIN PRINT 'Updating TicketId column size from varchar(20) to varchar(50)...'

-- Alter the TicketId column to accommodate GUID values
ALTER TABLE [dbo].[Feedbacks] 
    ALTER COLUMN [TicketId] [varchar](50) NULL

PRINT 'TicketId column updated successfully!' END ELSE BEGIN PRINT 'Feedbacks table or TicketId column not found.' END