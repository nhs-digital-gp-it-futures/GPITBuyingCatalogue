IF NOT EXISTS (SELECT * FROM dbo.CompliancyLevel)
    INSERT INTO dbo.CompliancyLevel(Id, [Name])
    VALUES
    (1, 'MUST'),
    (2, 'SHOULD'),
    (3, 'MAY');
GO
