DECLARE
@frameworks AS TABLE
(
    Id nvarchar(10) NOT NULL PRIMARY KEY,
    [Name] nvarchar(100) NOT NULL,
    ShortName nvarchar(25) NULL,
    [Owner] nvarchar(100) NULL,
    FundingTypes NVARCHAR(30) NULL,
    IsExpired BIT NOT NULL DEFAULT(0),
    SupportsFoundationSolution BIT NOT NULL DEFAULT(0)
);

INSERT INTO @frameworks (Id, [Name], ShortName, [Owner], FundingTypes, IsExpired, SupportsFoundationSolution)
VALUES ('NHSDGP001', 'NHS Digital GP IT Futures Framework 1', 'GP IT Futures', 'NHS Digital', '1,2', 1, 1),
       ('DFOCVC001', 'Digital First Online Consultation and Video Consultation Framework 1', 'DFOCVC', 'NHS England', '2', 0, 0),
       ('TIF001', 'Tech Innovation Framework', 'Tech Innovation', 'NHS Digital', '1', 0, 0),
       ('ATP001', 'Advanced Telephony Framework', 'Advanced Telephony', 'NHS England', '2', 0, 0);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    UPDATE @frameworks SET FundingTypes = '1,2' WHERE Id = 'TIF001'
END

MERGE INTO catalogue.Frameworks AS TARGET
    USING @frameworks AS SOURCE ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN
        UPDATE SET
            TARGET.[Name] = SOURCE.[Name],
            TARGET.ShortName = SOURCE.ShortName,
            TARGET.[Owner] = SOURCE.[Owner],
            TARGET.FundingTypes = SOURCE.FundingTypes,
            TARGET.IsExpired = SOURCE.IsExpired,
            TARGET.SupportsFoundationSolution = SOURCE.SupportsFoundationSolution
    WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name], ShortName, [Owner], FundingTypes, IsExpired, SupportsFoundationSolution)
    VALUES (SOURCE.Id, SOURCE.[Name], SOURCE.ShortName, SOURCE.[Owner], SOURCE.FundingTypes, SOURCE.IsExpired, SOURCE.SupportsFoundationSolution);
GO
