IF NOT EXISTS (SELECT * FROM catalogue.CapabilityStatus)
    INSERT INTO catalogue.CapabilityStatus(Id, [Name])
    VALUES (1, 'Effective');
GO
