MERGE INTO catalogue.StandardTypes AS TARGET
USING (
VALUES
    (1, 'Overarching'),
    (5, 'Other')
)
AS SOURCE([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name]
THEN UPDATE SET
TARGET.[Name] = SOURCE.[Name]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name])
VALUES (SOURCE.[Id], SOURCE.[Name]);
GO
