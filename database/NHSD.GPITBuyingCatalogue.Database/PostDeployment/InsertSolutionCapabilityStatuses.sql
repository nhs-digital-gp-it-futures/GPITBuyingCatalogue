MERGE INTO catalogue.CatalogueItemCapabilityStatus AS TARGET
USING (
VALUES 
    (1, 'Passed – Full', 1),
    (2, 'Passed – Partial', 1),
    (3, 'Failed', 0)
)
AS SOURCE ([Id], [Name], [Pass])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED 
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Pass] = SOURCE.[Pass]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [Pass])
VALUES (SOURCE.[Id], SOURCE.[Name],SOURCE.[Pass]);
GO
