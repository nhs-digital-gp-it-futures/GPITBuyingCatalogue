IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* SolutionCapability */
    /*********************************************************************************************************************************************/

    CREATE TABLE #CatalogueItemCapability
    (
        CatalogueItemId nvarchar(14) NOT NULL,
        CapabilityId int NOT NULL,
        StatusId int NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        LastUpdatedBy int NULL
    );

    DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
    DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);
    DECLARE @now AS datetime2 = GETUTCDATE();

    INSERT INTO #CatalogueItemCapability (CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy) 
         VALUES (N'10000-001', 5,  1, @now, @bobUser),
                (N'10000-001', 12, 1, @now, @bobUser),
                (N'10000-001', 30, 1, @now, @bobUser),
                (N'10000-001', 11, 1, @now, @bobUser),
                (N'10000-001', 10, 1, @now, @bobUser),
                (N'10000-001', 17, 1, @now, @bobUser),
                (N'10000-001', 16, 1, @now, @bobUser),
                (N'10000-001', 13, 1, @now, @bobUser),
                (N'10000-001', 15, 1, @now, @bobUser),
                (N'10000-001', 14, 1, @now, @bobUser),
                (N'10000-001', 41, 1, @now, @bobUser),
                (N'10000-001', 6,  1, @now, @bobUser),
                (N'10000-001', 20, 1, @now, @bobUser),
                (N'10000-002', 41, 1, @now, @bobUser),
                (N'10000-062', 41, 1, @now, @bobUser),
                (N'10004-001', 35, 1, @now, @bobUser),
                (N'10004-001', 34, 1, @now, @bobUser),
                (N'10004-001', 6,  1, @now, @bobUser),
                (N'10004-002', 5,  1, @now, @bobUser),
                (N'10007-002', 6,  1, @now, @bobUser),
                (N'10011-003', 41, 1, @now, @bobUser),
                (N'10029-003', 41, 1, @now, @bobUser),
                (N'10030-001', 41, 1, @now, @bobUser),
                (N'10031-001', 41, 1, @now, @bobUser),
                (N'10035-001', 3,  1, @now, @bobUser),
                (N'10035-001', 33, 1, @now, @bobUser),
                (N'10035-001', 1,  1, @now, @bobUser),
                (N'10035-001', 2,  1, @now, @bobUser),
                (N'10035-001', 4,  1, @now, @bobUser),
                (N'10046-001', 9,  1, @now, @bobUser),
                (N'10046-001', 17, 1, @now, @bobUser),
                (N'10046-001', 19, 1, @now, @bobUser),
                (N'10046-001', 20, 1, @now, @bobUser),
                (N'10046-003', 9,  1, @now, @bobUser),
                (N'10046-003', 17, 1, @now, @bobUser),
                (N'10046-006', 41, 1, @now, @bobUser),
                (N'10052-002', 3,  1, @now, @bobUser),
                (N'10052-002', 5,  1, @now, @bobUser),
                (N'10052-002', 12, 1, @now, @bobUser),
                (N'10052-002', 39, 1, @now, @bobUser),
                (N'10052-002', 11, 1, @now, @bobUser),
                (N'10052-002', 16, 1, @now, @bobUser),
                (N'10052-002', 13, 1, @now, @bobUser),
                (N'10052-002', 1,  1, @now, @bobUser),
                (N'10052-002', 2,  1, @now, @bobUser),
                (N'10052-002', 26, 1, @now, @bobUser),
                (N'10052-002', 36, 1, @now, @bobUser),
                (N'10052-002', 15, 1, @now, @bobUser),
                (N'10052-002', 14, 1, @now, @bobUser),
                (N'10052-002', 41, 1, @now, @bobUser),
                (N'10052-002', 4,  1, @now, @bobUser),
                (N'10059-001', 6,  1, @now, @bobUser),
                (N'10063-002', 41, 1, @now, @bobUser),
                (N'10063-002', 29, 1, @now, @bobUser),
                (N'10072-003', 41, 1, @now, @bobUser),
                (N'10072-004', 1,  1, @now, @bobUser),
                (N'10072-004', 41, 1, @now, @bobUser),
                (N'10073-009', 41, 1, @now, @bobUser),
                (N'10000-001A003',  41, 3, @now, @bobUser),
                (N'10000-001A003',  1,  1, @now, @bobUser),
                (N'10000-001A005',  41, 1, @now, @bobUser),
                (N'10000-001A005',  1,  3, @now, @bobUser),
                (N'10000-001A005',  6,  1, @now, @bobUser),
                (N'10000-001A005',  29, 3, @now, @bobUser),
                (N'10000-001A006',  4,  3, @now, @bobUser),
                (N'10000-001A006',  6,  1, @now, @bobUser);
    
    MERGE INTO catalogue.CatalogueItemCapabilities AS TARGET
    USING #CatalogueItemCapability AS SOURCE
    ON TARGET.CatalogueItemId = SOURCE.CatalogueItemId AND TARGET.CapabilityId = SOURCE.CapabilityId 
    WHEN MATCHED THEN
           UPDATE SET TARGET.StatusId = SOURCE.StatusId,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.LastUpdatedBy = SOURCE.LastUpdatedBy
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
        VALUES (SOURCE.CatalogueItemId, SOURCE.CapabilityId, SOURCE.StatusId, SOURCE.LastUpdated, SOURCE.LastUpdatedBy);
END;
GO
