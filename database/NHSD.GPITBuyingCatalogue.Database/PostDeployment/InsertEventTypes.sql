MERGE INTO [notifications].[EventTypes] AS TARGET
USING (
VALUES 
    (1, 'OrderEnteredFirstExpiryThreshold', 1),
    (2, 'OrderEnteredSecondExpiryThreshold', 1),
    (3, 'PasswordEnteredFirstExpiryThreshold', 2),
    (4, 'PasswordEnteredSecondExpiryThreshold', 2),
    (5, 'PasswordEnteredThirdExpiryThreshold', 2)
)

AS SOURCE ([Id], [Name], [EmailPreferenceTypeId])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED
THEN UPDATE SET
    TARGET.[Name] = SOURCE.[Name],
    TARGET.[EmailPreferenceTypeId] = SOURCE.[EmailPreferenceTypeId]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [EmailPreferenceTypeId])
VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[EmailPreferenceTypeId]);
GO

