IF NOT EXISTS (SELECT * FROM catalogue.CataloguePriceTypes)
    INSERT INTO catalogue.CataloguePriceTypes(Id, [Name])
    VALUES
    (1, 'Flat'),
    (2, 'Tiered');
GO
