MERGE INTO catalogue.CataloguePriceQuantityCalculationTypes AS TARGET
USING (
VALUES 
    (1, 'PerSolutionOrService'),
    (2, 'PerServiceRecipient')
)
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED 
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (SOURCE.[Id], SOURCE.[Name]);
GO
