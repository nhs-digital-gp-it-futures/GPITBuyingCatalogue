MERGE INTO catalogue.CatalogueItemTypes AS TARGET
USING (
VALUES 
    (1, 'Solution'),
    (2, 'Additional Service'),
    (3, 'Associated Service')
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
