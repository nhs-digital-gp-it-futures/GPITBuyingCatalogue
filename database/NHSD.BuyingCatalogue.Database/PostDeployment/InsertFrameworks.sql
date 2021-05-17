DECLARE @frameworks AS TABLE
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    ShortName nvarchar(25) NULL,
    [Owner] nvarchar(100) NULL
);

INSERT INTO @frameworks (Id, [Name], ShortName, [Owner])
VALUES
('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'GP IT Futures', 'NHS Digital'),
('DFOCVC001', 'Digital First Online Consultation and Video Consultation Framework 1', 'DFOCVC', 'NHS England');

MERGE INTO dbo.Framework AS TARGET
     USING @frameworks AS SOURCE ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
           UPDATE SET TARGET.[Name] = SOURCE.[Name]
      WHEN NOT MATCHED BY TARGET THEN
           INSERT (Id, [Name], ShortName, [Owner])
           VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.ShortName, SOURCE.[Owner]);
GO
