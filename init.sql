IF NOT EXISTS (SELECT name
FROM sys.databases
WHERE name = 'ECommerceBadmintonDB')
BEGIN
    CREATE DATABASE ECommerceBadmintonDB;
END
GO

USE ECommerceBadmintonDB;
GO
