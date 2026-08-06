MERGE INTO [notifications].[EmailNotificationTypes] AS TARGET
USING (
VALUES 
    (1, 'ContractDueToExpire'),
    (2, 'PasswordDueToExpire'),
    (3, 'InactiveAccount'),
    (4, 'AccountDeactivation')
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
