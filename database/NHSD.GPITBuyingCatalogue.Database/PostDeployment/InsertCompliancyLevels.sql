IF NOT EXISTS (SELECT * FROM catalogue.CompliancyLevels)
    INSERT INTO catalogue.CompliancyLevels(Id, [Name])
    VALUES
    (1, 'MUST'),
    (2, 'SHOULD'),
    (3, 'MAY');
GO
