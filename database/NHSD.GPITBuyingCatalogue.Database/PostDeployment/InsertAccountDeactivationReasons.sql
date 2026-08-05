MERGE INTO [users].[AccountDeactivationReasons] AS TARGET
USING (
VALUES 
    (0, 'Manual'),
    (1, 'Inactivity')
)

AS SOURCE ([Id], [Reason])
ON TARGET.[Id] = SOURCE.[Id]

WHEN MATCHED
THEN UPDATE SET
    TARGET.[Reason] = SOURCE.[Reason]

WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Reason])
VALUES (SOURCE.[Id], SOURCE.[Reason]);
GO
