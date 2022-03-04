CREATE USER [hangfire] FOR LOGIN [hangfire] WITH DEFAULT_SCHEMA = [HangFire];
GO

GRANT CONNECT TO [hangfire];
GO

ALTER ROLE [BackgroundProcessor] ADD MEMBER [hangfire];
GO
