IF NOT EXISTS (SELECT * FROM catalogue.CataloguePriceCalculationTypes)
    INSERT INTO catalogue.CataloguePriceCalculationTypes(Id, [Name])
    VALUES
    (1, 'Cumilative'),
    (2, 'SingleFixed');
GO
