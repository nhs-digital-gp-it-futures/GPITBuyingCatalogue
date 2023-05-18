DECLARE
@frameworks AS TABLE
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    ShortName nvarchar(25) NULL,
    [Owner] nvarchar(100) NULL,
    LocalFundingOnly BIT NULL,
    IsExpired BIT NOT NULL DEFAULT(0)
);

INSERT INTO @frameworks (Id, [Name], ShortName, [Owner], LocalFundingOnly, IsExpired)
VALUES ('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'GP IT Futures', 'NHS Digital', 0, 1),
       ('DFOCVC001', 'Digital First Online Consultation and Video Consultation Framework 1', 'DFOCVC', 'NHS England', 1, 0),
       ('TIF001', 'Tech Innovation Framework', 'Tech Innovation', 'NHS Digital', 0, 0),
       ('ATP001', 'Advanced Telephony Framework', 'Advanced Telephony', 'NHS England', 1, 0);

MERGE INTO catalogue.Frameworks AS TARGET
    USING @frameworks AS SOURCE ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET
            TARGET.[Name] = SOURCE.[Name],
            TARGET.ShortName = SOURCE.ShortName,
            TARGET.[Owner] = SOURCE.[Owner],
            TARGET.LocalFundingOnly = SOURCE.LocalFundingOnly,
            TARGET.IsExpired = SOURCE.IsExpired
    WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name], ShortName, [Owner], LocalFundingOnly, IsExpired)
    VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.ShortName, SOURCE.[Owner], SOURCE.LocalFundingOnly, SOURCE.IsExpired);
GO
