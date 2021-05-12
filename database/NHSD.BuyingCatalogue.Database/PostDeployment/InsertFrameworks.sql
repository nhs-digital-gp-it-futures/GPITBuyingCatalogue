IF NOT EXISTS (SELECT * FROM dbo.Framework)
    INSERT INTO dbo.Framework(Id, [Name], [Owner])
    VALUES ('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'NHS Digital');
GO
