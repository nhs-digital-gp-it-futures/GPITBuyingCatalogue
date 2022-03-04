CREATE ROLE [BackgroundProcessor];
GO

GRANT CONTROL ON SCHEMA::[HangFire] TO [BackgroundProcessor]
GO

GRANT CREATE TABLE TO [BackgroundProcessor]
GO
