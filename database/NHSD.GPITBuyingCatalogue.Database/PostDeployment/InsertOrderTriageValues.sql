MERGE INTO ordering.OrderTriageValues AS TARGET
USING (
VALUES 
    (0, 'Under40K'),
    (1, 'Between40KTo250K'),
    (2, 'Over250K')
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

