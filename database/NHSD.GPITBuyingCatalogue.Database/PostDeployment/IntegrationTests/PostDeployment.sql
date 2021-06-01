-- This post-deployment script is only executed when deploying the integration test environment.
-- i.e. When the INTEGRATION_TEST environment variable is set to 'TRUE'.

IF NOT EXISTS (SELECT * FROM information_schema.schemata WHERE schema_name = 'test')
    EXEC sp_executesql N'CREATE SCHEMA test;'
GO

:r ./ClearData.sql
:r ./DropRole.sql
:r ./DropUserAndLogin.sql
:r ./RestoreUserAndLogin.sql
