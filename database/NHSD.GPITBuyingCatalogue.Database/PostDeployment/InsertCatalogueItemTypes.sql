IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItemTypes)
    INSERT INTO catalogue.CatalogueItemTypes(Id, [Name])
    VALUES
    (1, 'Solution'),
    (2, 'Additional Service'),
    (3, 'Associated Service');
GO
