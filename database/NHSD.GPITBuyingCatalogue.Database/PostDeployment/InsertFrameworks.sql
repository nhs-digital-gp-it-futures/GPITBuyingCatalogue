DECLARE @frameworks AS TABLE
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    ShortName nvarchar(25) NULL,
    [Owner] nvarchar(100) NULL,
    LocalFundingOnly BIT NULL
);

INSERT INTO @frameworks (Id, [Name], ShortName, [Owner], LocalFundingOnly)
VALUES
('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'GP IT Futures', 'NHS Digital', 0),
('DFOCVC001', 'Digital First Online Consultation and Video Consultation Framework 1', 'DFOCVC', 'NHS England', 1),
('TIF001', 'Tech Innovation Framework', 'Tech Innovation', 'NHS Digital', 0),
('ATP001', 'Advanced Telephony Framework', 'Advanced Telephony', 'NHS England', 1);

MERGE INTO catalogue.Frameworks AS TARGET
     USING @frameworks AS SOURCE ON TARGET.Id = SOURCE.Id
      WHEN MATCHED THEN
           UPDATE SET
            TARGET.[Name] = SOURCE.[Name],
            TARGET.ShortName = SOURCE.ShortName,
            TARGET.[Owner] = SOURCE.[Owner],
            TARGET.LocalFundingOnly = SOURCE.LocalFundingOnly
      WHEN NOT MATCHED BY TARGET THEN
           INSERT (Id, [Name], ShortName, [Owner], LocalFundingOnly)
           VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.ShortName, SOURCE.[Owner], SOURCE.LocalFundingOnly);
GO
