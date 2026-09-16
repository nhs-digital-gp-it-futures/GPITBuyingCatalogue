DECLARE @userName sysname = N'buyingcatalogue-reportingservice@nhs.net';
DECLARE @sql nvarchar(max);

-- Create the Microsoft Entra Application Account database user if absent.
IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_principals
    WHERE name = @userName
)
BEGIN
    SET @sql = N'CREATE USER ' + QUOTENAME(@userName) + N' FROM EXTERNAL PROVIDER;';
    EXEC sys.sp_executesql @sql;
END;

-- Add the user to db_datareader.
IF NOT EXISTS
(
    SELECT 1
    FROM sys.database_role_members AS drm
    INNER JOIN sys.database_principals AS role_principal
        ON role_principal.principal_id = drm.role_principal_id
    INNER JOIN sys.database_principals AS member_principal
        ON member_principal.principal_id = drm.member_principal_id
    WHERE role_principal.name = N'db_datareader'
      AND member_principal.name = @userName
)
BEGIN
    SET @sql = N'ALTER ROLE [db_datareader] ADD MEMBER ' + QUOTENAME(@userName) + N';';
    EXEC sys.sp_executesql @sql;
END;
GO