MERGE INTO catalogue.SolutionStandardStatuses AS TARGET
USING (VALUES (1, 'N/A'),
              (2, 'Not Met'),
              (3, 'Not Yet Selected'),
              (4, 'In Progress'),
              (5, 'Fully Met'))
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name])
    VALUES ([Id], [Name])
WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name]
THEN UPDATE SET
                TARGET.[Name] = SOURCE.[Name];
GO
