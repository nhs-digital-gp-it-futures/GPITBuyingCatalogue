MERGE INTO catalogue.SolutionSectionRequirements AS TARGET
USING (VALUES (0, 'Not Applicable'),
              (1, 'Optional'),
              (2, 'Mandatory'))
AS SOURCE ([Id], [Name])
ON TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name])
    VALUES ([Id], [Name])
WHEN MATCHED AND TARGET.[Name] <> SOURCE.[Name]
THEN UPDATE SET
                TARGET.[Name] = SOURCE.[Name];
GO
