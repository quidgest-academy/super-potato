-- ================================================
--              DISABLE CHANGE TRACKING
-- ================================================
-- This disables change tracking for each table
-- and then for the database.
-- ================================================

USE [W_GnBD]
GO

-- First generate and execute ALTER statements for all tables with change tracking enabled
DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql += 'ALTER TABLE [' + OBJECT_SCHEMA_NAME(ct.object_id) + '].[' + OBJECT_NAME(ct.object_id) + '] DISABLE CHANGE_TRACKING;' + CHAR(13)
FROM sys.change_tracking_tables ct;

IF @sql > ''
BEGIN
    EXEC sp_executesql @sql;
END

-- Then disable change tracking at the database level if enabled
IF EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID('W_GnBD'))
BEGIN
    ALTER DATABASE [W_GnBD]
    SET CHANGE_TRACKING = OFF;
END
