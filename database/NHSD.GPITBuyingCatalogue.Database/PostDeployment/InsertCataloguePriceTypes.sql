MERGE INTO catalogue.CataloguePriceTypes AS TARGET
USING (
VALUES 
    (1, 'Flat'),
    (2, 'Tiered')
)
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name] 
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (SOURCE.[Id], SOURCE.[Name]);
GO
