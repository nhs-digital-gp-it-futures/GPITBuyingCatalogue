MERGE INTO [notifications].[EmailPreferenceTypes] AS TARGET
USING (
VALUES 
    (1, 'ContractExpiry', 1)
)
AS SOURCE ([Id], [Name], [DefaultEnabled])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED
THEN UPDATE SET
    TARGET.[Name] = SOURCE.[Name],
    TARGET.[DefaultEnabled] = SOURCE.[DefaultEnabled]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [DefaultEnabled])
VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[DefaultEnabled]);
GO
