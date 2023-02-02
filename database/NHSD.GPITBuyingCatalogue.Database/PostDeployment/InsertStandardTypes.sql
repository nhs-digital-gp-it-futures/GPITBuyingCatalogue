IF NOT EXISTS (SELECT * FROM catalogue.StandardTypes)
    INSERT INTO catalogue.StandardTypes(Id, [Name])
    VALUES
    (1, 'Overarching'),
    (2, 'Interoperability'),
    (3, 'Capability'),
    (4, 'ContextSpecific');
GO
