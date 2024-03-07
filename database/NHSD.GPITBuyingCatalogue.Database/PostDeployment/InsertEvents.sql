MERGE INTO [notifications].[Events] AS TARGET
USING (
VALUES 
    (1, 'OrderCompleted', null),
    (2, 'OrderExpiryDueFirstThreshold', 1),
    (3, 'OrderExpiryDueSecondThreshold', 1),
    (4, 'UserPasswordExpired', 2)
)

AS SOURCE ([Id], [Name], [ManagedEmailPreferenceId])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED
THEN UPDATE SET
    TARGET.[Name] = SOURCE.[Name],
    TARGET.[ManagedEmailPreferenceId] = SOURCE.[ManagedEmailPreferenceId]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [ManagedEmailPreferenceId])
VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[ManagedEmailPreferenceId]);
GO

