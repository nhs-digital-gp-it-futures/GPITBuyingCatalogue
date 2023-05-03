MERGE INTO catalogue.ClientApplicationTypes AS TARGET
USING (
VALUES 
    (1, 'browser-based', 'Browser-based'),
    (2, 'native-mobile', 'Mobile or tablet'),
    (3, 'native-desktop', 'Desktop')
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
