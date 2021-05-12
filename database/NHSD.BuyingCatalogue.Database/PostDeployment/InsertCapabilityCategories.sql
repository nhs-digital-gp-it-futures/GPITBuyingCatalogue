DECLARE @capabilityCategories AS TABLE
(
    Id int NOT NULL,
    [Name] nvarchar(50) NOT NULL
);

INSERT INTO @capabilityCategories(Id, [Name])
VALUES
(0, 'Undefined'),
(1, 'GP IT Futures'),
(2, 'Covid-19 Vaccination');

MERGE INTO dbo.CapabilityCategory AS TARGET
     USING @capabilityCategories AS SOURCE
        ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
UPDATE SET TARGET.[Name] = SOURCE.[Name]
      WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name])
    VALUES (SOURCE.Id, SOURCE.[Name]);
GO
