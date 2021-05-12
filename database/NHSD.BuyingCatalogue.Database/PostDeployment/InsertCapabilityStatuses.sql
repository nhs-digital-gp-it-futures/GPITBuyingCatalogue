IF NOT EXISTS (SELECT * FROM dbo.CapabilityStatus)
    INSERT INTO dbo.CapabilityStatus(Id, [Name])
    VALUES (1, 'Effective');
GO
