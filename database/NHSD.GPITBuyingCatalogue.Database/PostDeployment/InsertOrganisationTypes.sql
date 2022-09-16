MERGE INTO organisations.OrganisationTypes AS TARGET
USING (
VALUES 
(1, 'Clinical Commissioning Group', 'CG'),
(2, 'Executive Agency', 'EA')
)
AS SOURCE ([Id], [Name], [Identifier])
ON TARGET.[Identifier] = SOURCE.[Identifier]

WHEN MATCHED 
THEN UPDATE SET
TARGET.[Id] = SOURCE.[Id],
TARGET.[Name] = SOURCE.[Name]

    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [Identifier])
VALUES (SOURCE.[Id], SOURCE.[Name],SOURCE.[Identifier]);
GO
