MERGE INTO [notifications].[EmailPreferenceRoleTypes] AS TARGET
USING (
VALUES 
    (1, 'All'),
    (2, 'Admins'),
    (3, 'Buyers')
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
