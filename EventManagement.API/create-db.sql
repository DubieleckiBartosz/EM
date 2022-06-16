IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'EventManagement')
BEGIN
    CREATE DATABASE EventManagement
END
    
GO

IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'EventManagementTests')
BEGIN
    CREATE DATABASE EventManagementTests
END
    
GO
