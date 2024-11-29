IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE'
BEGIN
    DECLARE 
        @CatalogueSolution1Id NVARCHAR(14) = '99998-98', -- NotSystmOne
        @CatalogueSolution2Id NVARCHAR(14) = '99999-01', -- DFOCVC Online Consultation
        @CatalogueSolution3Id NVARCHAR(14) = '99999-02', -- GPIT DFOCVC Online Consultation
        @CatalogueSolution4Id NVARCHAR(14) = '99999-89', -- NotEmis Web GP
        @OverarchingStandardId NVARCHAR(5) = 'S28', -- Training
        @SupplementaryCareStandardId NVARCHAR(5) = 'SCS1'
    -- Community Pharmacy Clinical Services

    DECLARE @WorkOffPlans AS TABLE
        (
        [Id] INT NOT NULL PRIMARY KEY,
        [SolutionId] NVARCHAR(14) NOT NULL,
        [StandardId] NVARCHAR(5) NOT NULL,
        [Details] NVARCHAR(300) NOT NULL,
        [CompletionDate] date DEFAULT '2019-12-31' NOT NULL
        )

    INSERT INTO @WorkOffPlans
        ([Id], [SolutionId], [StandardId], [Details], [CompletionDate])
    VALUES
        (1, @CatalogueSolution1Id, @OverarchingStandardId, 'Needs more work to implement', DATEADD(day, 187, SYSDATETIME())),
        (2, @CatalogueSolution2Id, @SupplementaryCareStandardId, 'Needs more work to implement', DATEADD(day, 362, SYSDATETIME())),
        (3, @CatalogueSolution3Id, @OverarchingStandardId, 'Needs more work to implement', DATEADD(day, 90, SYSDATETIME())),
        (4, @CatalogueSolution4Id, @SupplementaryCareStandardId, 'Needs more work to implement', DATEADD(day, 233, SYSDATETIME()))


    SET IDENTITY_INSERT catalogue.WorkOffPlans ON

    MERGE INTO catalogue.WorkOffPlans AS TARGET
        USING @WorkOffPlans AS SOURCE
            ON TARGET.Id = SOURCE.Id
        WHEN MATCHED THEN
            UPDATE SET TARGET.[SolutionId] = SOURCE.[SolutionId],
                        TARGET.[StandardId] = SOURCE.[StandardId],
                        TARGET.[Details] = SOURCE.[Details],
                        TARGET.[CompletionDate] = SOURCE.[CompletionDate]
        WHEN NOT MATCHED THEN
        INSERT (Id, [SolutionId], [StandardId], [Details], [CompletionDate])
        VALUES(SOURCE.Id, SOURCE.[SolutionId], SOURCE.[StandardId], SOURCE.[Details], SOURCE.[CompletionDate]);

    SET IDENTITY_INSERT catalogue.WorkOffPlans OFF
END