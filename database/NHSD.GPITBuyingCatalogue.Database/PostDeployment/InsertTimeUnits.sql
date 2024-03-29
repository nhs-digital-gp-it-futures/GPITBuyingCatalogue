﻿MERGE INTO catalogue.TimeUnits AS TARGET
USING (
VALUES 
    (1, 'month', 'per month'),
    (2, 'year', 'per year')
)
AS SOURCE ([Id], [Name], [Description])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name] OR TARGET.[Description] <> SOURCE.[Description]
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Description] = SOURCE.[Description]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [Description])
VALUES (SOURCE.[Id], SOURCE.[Name],SOURCE.[Description]);
GO
