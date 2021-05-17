IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* AdditionalService */
    /*********************************************************************************************************************************************/

    CREATE TABLE #AdditionalService
    (
        CatalogueItemId nvarchar(14) NOT NULL,
        Summary nvarchar(300) NULL,
        FullDescription nvarchar(3000) NULL,
        LastUpdated datetime2(7) NULL,
        LastUpdatedBy uniqueidentifier NULL,
        SolutionId nvarchar(14) NULL,
    );

    INSERT INTO #AdditionalService (CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId) 
         VALUES (N'10030-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10030-001'),
                (N'10007-002A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10007-002'),
                (N'10007-002A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10007-002'),
                (N'10000-001A008', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A007', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A006', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A005', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A003', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A004', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10000-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10000-001'),
                (N'10035-001A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10035-001'),
                (N'10052-002A001', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A002', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A004', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A003', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002'),
                (N'10052-002A005', N'SUMMARY', N'DESCRIPTION', GETUTCDATE(), N'00000000-0000-0000-0000-000000000000', N'10052-002');

    MERGE INTO dbo.AdditionalService AS TARGET
    USING #AdditionalService AS SOURCE
    ON TARGET.CatalogueItemId = SOURCE.CatalogueItemId 
    WHEN MATCHED THEN  
           UPDATE SET TARGET.Summary = SOURCE.Summary,
                      TARGET.FullDescription = SOURCE.FullDescription,
                      TARGET.SolutionId = SOURCE.SolutionId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN  
        INSERT (CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId) 
        VALUES (SOURCE.CatalogueItemId, SOURCE.Summary, SOURCE.FullDescription, SOURCE.LastUpdated, SOURCE.LastUpdatedBy, SOURCE.SolutionId);
END;
GO
