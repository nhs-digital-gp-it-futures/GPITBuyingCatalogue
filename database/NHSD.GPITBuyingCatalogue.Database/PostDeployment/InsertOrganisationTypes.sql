MERGE INTO organisations.OrganisationTypes AS TARGET
USING (
VALUES 
    (1, 'Clinical Commissioning Group', 'CG'),
    (2, 'Executive Agency', 'EA'),
    (3, 'Integrated Care Board', 'IB'),
    (4, 'GP Practice', 'GP'),
    (5, 'Commissioning Support Unit', 'CS')
)
AS SOURCE ([Id], [Name], [Identifier])
ON TARGET.[Identifier] = SOURCE.[Identifier]

WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name] OR TARGET.[Id] <> SOURCE.[Id]
THEN UPDATE SET
TARGET.[Id] = SOURCE.[Id],
TARGET.[Name] = SOURCE.[Name]
    
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Name], [Identifier])
VALUES (SOURCE.[Id], SOURCE.[Name],SOURCE.[Identifier]);
GO
