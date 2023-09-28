MERGE INTO ordering.OrderItemFundingTypes AS TARGET
USING (
VALUES 
    (0, 'None'),
    (1, 'Central'),
    (2, 'Local'),
    (3, 'Mixed'),
    (4, 'No Funding Required'),
    (5, 'Local Funding Only'),
    (6, 'GPIT'),
    (7, 'PCARP')
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
