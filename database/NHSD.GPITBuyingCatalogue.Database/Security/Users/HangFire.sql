CREATE USER [HangFire] FOR LOGIN [HangFire] WITH DEFAULT_SCHEMA = [HangFire];
GO

GRANT CONNECT TO [HangFire];
GO

ALTER ROLE [BackgroundProcessor] ADD MEMBER [HangFire];
GO
