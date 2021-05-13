CREATE ROLE Api;
GO

ALTER ROLE db_datareader
ADD MEMBER Api;
GO

ALTER ROLE db_datawriter
ADD MEMBER Api;
GO
