MERGE INTO catalogue.CompliancyLevels AS TARGET
USING (
VALUES 
    (1, 'MUST'),
    (2, 'SHOULD'),
    (3, 'MAY')
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
