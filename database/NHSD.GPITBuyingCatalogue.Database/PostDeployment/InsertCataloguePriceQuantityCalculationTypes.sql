IF NOT EXISTS (SELECT * FROM catalogue.CataloguePriceQuantityCalculationTypes)
    INSERT INTO catalogue.CataloguePriceQuantityCalculationTypes(Id, [Name])
    VALUES
    (1, 'PerSolutionOrService'),
    (2, 'PerServiceRecipient');
GO
