MERGE INTO [notifications].[EmailPreferenceTypes] AS TARGET
USING (
VALUES 
    (1, 'ContractExpiry', 1, 3),
    (2, 'PasswordExpiry', 1, 1)
)
AS SOURCE ([Id], [Name], [DefaultEnabled], [RoleType])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED
THEN UPDATE SET
    TARGET.[Name] = SOURCE.[Name],
    TARGET.[DefaultEnabled] = SOURCE.[DefaultEnabled],
    TARGET.[RoleType] = SOURCE.[RoleType]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [DefaultEnabled], [RoleType])
VALUES (SOURCE.[Id], SOURCE.[Name], SOURCE.[DefaultEnabled], SOURCE.[RoleType]);
GO
