IF NOT EXISTS (SELECT * FROM catalogue.CataloguePriceCalculationTypes)
    INSERT INTO catalogue.CataloguePriceCalculationTypes(Id, [Name])
    VALUES
    (1, 'SingleFixed'),
    (2, 'Cumilative'),
    (3, 'Volume');
GO
