IF NOT EXISTS (SELECT * FROM catalogue.CataloguePriceTypes)
    INSERT INTO catalogue.CataloguePriceTypes(CataloguePriceTypeId, [Name])
    VALUES
    (1, 'Flat'),
    (2, 'Tiered');
GO
