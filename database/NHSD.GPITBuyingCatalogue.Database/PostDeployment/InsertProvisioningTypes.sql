MERGE INTO catalogue.ProvisioningTypes AS TARGET
USING (
VALUES 
    (1, 'Patient'),
    (2, 'Declarative'),
    (3, 'OnDemand'),
    (4, 'PerServiceRecipient')
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
