MERGE INTO catalogue.HostingTypes AS TARGET
USING (
VALUES 
    (1, 'PublicCloud', 'Public cloud'),
    (2, 'PrivateCloud', 'Private cloud'),
    (3, 'HybridHostingType', 'Hybrid'),
    (4, 'OnPremise', 'On premise')
)
AS SOURCE ([Id], [Name], [Description])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name] OR TARGET.[Description] <> SOURCE.[Description] 
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Description] = SOURCE.[Description]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [Description])
VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[Description]);
GO
