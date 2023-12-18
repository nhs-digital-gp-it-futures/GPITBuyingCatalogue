IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    /*********************************************************************************************************************************************/
    /* CataloguePrice */
    /*********************************************************************************************************************************************/

    DECLARE @CataloguePrice TABLE
    (
        CataloguePriceId int NOT NULL,
        CatalogueItemId nvarchar(14) NOT NULL,
        ProvisioningTypeId int NOT NULL,
        CataloguePriceTypeId int NOT NULL,
        PricingUnitId smallint NOT NULL,
        TimeUnitId int NULL,
        CataloguePriceCalculationTypeId INT NOT NULL,
        CataloguePriceQuantityCalculationTypeId INT NULL,
        CurrencyCode nvarchar(3) NOT NULL,
        LastUpdated datetime2(7) NOT NULL,
        Price decimal(18,3) NULL,
        PublishedStatusId INT NOT NULL
    );

    DECLARE @patient AS smallint = -1;
    DECLARE @consultation AS smallint = -3;
    DECLARE @licence AS smallint = -4;
    DECLARE @halfDay AS smallint = -6;
    DECLARE @course AS smallint = -8;
    DECLARE @practice AS smallint = -9;
    DECLARE @day AS smallint = -10;
    DECLARE @activeUser AS smallint = -11;
    DECLARE @consultationCore AS smallint = -12;
    DECLARE @consultationExt AS smallint = -13;
    DECLARE @consultationOut AS smallint = -14;
    DECLARE @systmOneUnit AS smallint = -15;
    DECLARE @item AS smallint = -16;
    DECLARE @smsFragment AS smallint = -17;
    DECLARE @hourSession AS smallint = -18;
    DECLARE @extract AS smallint = -19;
    DECLARE @document AS smallint = -20;
    DECLARE @implementation AS smallint = -21;
    DECLARE @installation AS smallint = -22;
    DECLARE @migration AS smallint = -23;
    DECLARE @practiceMergerSplit AS smallint = -24;
    DECLARE @seminar AS smallint = -25;
    DECLARE @trainingEnvironment AS smallint = -26;
    DECLARE @unitMerge AS smallint = -27;
    DECLARE @unitSplit AS smallint = -28;
    DECLARE @user AS smallint = -29;

    -- Solutions per patient
    INSERT INTO @CataloguePrice (CataloguePriceId, CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, Price, PublishedStatusId) 
         VALUES (1001, N'10000-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 1.26, 3),
                (1002, N'10000-054', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 5.15, 3),
                (1003, N'10000-062', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 5.02, 3),
                (1004, N'10004-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.22, 3),
                (1005, N'10004-002', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.19, 3),
                (1006, N'10007-002', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.14, 3),
                (1007, N'10020-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.5, 3),
                (1008, N'10029-003', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.84, 3),
                (1009, N'10046-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.28, 3),
                (1010, N'10046-003', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.28, 3),
                (1011, N'10047-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.84, 3),
                (1012, N'10052-002', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 1.26, 3),
                (1013, N'10059-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.14, 3),
                (1014, N'10030-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 10.0, 3),
                (1015, N'10033-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 1.26, 3),
                (1016, N'10062-001', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.3, 3),
                (1094, N'10063-002', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.3, 3),
                (1095, N'10072-003', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.7, 3),                
                (1099, N'10072-006', 1, 1, @patient, 2, 1, NULL, 'GBP', GETUTCDATE(), 0.20, 3),                
    -- Solutions Variable On Demand 
                (1017, N'10035-001', 3, 1, @activeUser, NULL, 1, NULL, 'GBP', GETUTCDATE(), 0, 3),
                (1096, N'10072-004', 3, 1, @consultationCore, NULL, 1, NULL, 'GBP', GETUTCDATE(), 18, 3),
                (1097, N'10072-004', 3, 1, @consultationExt, NULL, 1, NULL, 'GBP', GETUTCDATE(), 20, 3),
                (1098, N'10072-004', 3, 1, @consultationOut, NULL, 1, NULL, 'GBP', GETUTCDATE(), 30, 3),
    -- Solutions Declarative
                (1018, N'10000-002', 2, 1, @licence, 1, 1, NULL, 'GBP', GETUTCDATE(), 37.92, 3),
                (1019, N'10073-009', 2, 1, @practice, 1, 1, NULL, 'GBP', GETUTCDATE(),207.92, 3),
    -- Additional Service Per Patient
                (1020, N'10000-001A001', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.25, 3),
                (1021, N'10000-001A002', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.07, 3),
                (1022, N'10000-001A004', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.06, 3),
                (1023, N'10000-001A005', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.25, 3),
                (1024, N'10000-001A006', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.12, 3),
                (1025, N'10000-001A007', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.06, 3),
                (1026, N'10000-001A008', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.06, 3),
                (1027, N'10007-002A001', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.04, 3),
                (1028, N'10007-002A002', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.05, 3),
                (1029, N'10030-001A001', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.20, 3),
                (1100, N'10052-002A001', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.20, 3),
                (1101, N'10052-002A002', 1, 1, @patient, 2, 1, NULL, 'GBP',GETUTCDATE(), 0.25, 3),
    -- Additional Service Variable
                (1031, N'10035-001A001', 3, 1, @consultation, NULL, 1, NULL, 'GBP', GETUTCDATE(), 10, 3),
    -- Additional Service Declarative
                (1033, N'10000-001A003', 2, 1, @licence, 1, 1, NULL, 'GBP', GETUTCDATE(),35.51, 3),
                (1034, N'10052-002A002', 2, 1, @systmOneUnit, 1, 1, NULL, 'GBP', GETUTCDATE(),68.5, 3),
                (1035, N'10052-002A003', 2, 1, @systmOneUnit, 1, 1, NULL, 'GBP', GETUTCDATE(),68.5, 3),
                (1036, N'10052-002A004', 2, 1, @systmOneUnit, 1, 1, NULL, 'GBP', GETUTCDATE(),291.67, 3),
                (1032, N'10052-002A005', 2, 1, @systmOneUnit, 1, 1, NULL, 'GBP', GETUTCDATE(),0, 3),
    -- Associated Service Variable
                (1037, N'10000-S-037', 3, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 4.35, 3),
                (1038, N'10000-S-038', 3, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1.25, 3),
                (1039, N'10047-S-002', 3, 1, @patient, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1.06, 3),
                (1102, N'10030-S-001', 3, 1, @smsFragment, NULL, 1, NULL, 'GBP', GETUTCDATE(), 0.017, 3),
    -- Associated Service Declarative
      --per 1hr session 
                (1040, N'10073-S-022', 2, 1, @hourSession, NULL, 1, NULL, 'GBP', GETUTCDATE(), 75, 3),
      --per Course
                (1041, N'10052-S-003', 2, 1, @course, NULL, 1, NULL, 'GBP', GETUTCDATE(), 3188.72, 3),
                (1042, N'10052-S-004', 2, 1, @course, NULL, 1, NULL, 'GBP', GETUTCDATE(), 6377.44, 3),
      --per data extraction
                (1043, N'10052-S-011', 2, 1, @extract, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1415.13, 3),
      --per day
                (1044, N'10000-S-036', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 595, 3),
                (1045, N'10000-S-041', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 510, 3),
                (1046, N'10000-S-003', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 650, 3),
                (1047, N'10004-S-002', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 500, 3),
                (1048, N'10004-S-001', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 500, 3),
                (1049, N'10007-S-002', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 300, 3),
                (1050, N'10007-S-004', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 405, 3),
                (1051, N'10007-S-005', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 495, 3),
                (1052, N'10029-S-006', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1053, N'10029-S-007', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1054, N'10029-S-008', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1055, N'10029-S-009', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1056, N'10029-S-010', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1057, N'10046-S-005', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1058, N'10046-S-006', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1059, N'10046-S-007', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1060, N'10046-S-009', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1061, N'10046-S-010', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1062, N'10046-S-001', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 925, 3),
                (1063, N'10046-S-002', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1064, N'10046-S-003', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 700, 3),
                (1065, N'10046-S-004', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 750, 3),
                (1066, N'10052-S-001', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 637.74, 3),
                (1067, N'10073-S-021', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 600, 3),
                (1068, N'10073-S-023', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 600, 3),
                (1103, N'10063-S-004', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
                (1104, N'10063-S-005', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
                (1105, N'10063-S-006', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
                (1106, N'10063-S-007', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
                (1107, N'10063-S-009', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
                (1108, N'10063-S-010', 2, 1, @day, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
      --per document
                (1069, N'10007-S-001', 2, 1, @document, NULL, 1, NULL, 'GBP', GETUTCDATE(), 20, 3),
      --per extract
                (1070, N'10052-S-012', 2, 1, @extract, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1979.21, 3),
      --per half day
                (1071, N'10000-S-004', 2, 1, @halfDay, NULL, 1, NULL, 'GBP', GETUTCDATE(), 252.5, 3),
                (1072, N'10052-S-002', 2, 1, @halfDay, NULL, 1, NULL, 'GBP', GETUTCDATE(), 637.74, 3),
                (1109, N'10063-S-008', 2, 1, @halfDay, NULL, 1, NULL, 'GBP', GETUTCDATE(), 630, 3),
      --per implementation
                (1073, N'10000-S-039', 2, 1, @implementation, NULL, 1, NULL, 'GBP', GETUTCDATE(), 3000, 3),
      --per installation
                (1074, N'10000-S-141', 2, 1, @installation, NULL, 1, NULL, 'GBP', GETUTCDATE(), 800, 3),
                (1075, N'10000-S-002', 2, 1, @installation, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1000, 3),
                (1076, N'10000-S-005', 2, 1, @installation, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2500, 3),
      --per item
                (1077, N'10000-S-009', 2, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1680, 3),
                (1078, N'10000-S-001', 2, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 90, 3),
                (1079, N'10000-S-006', 2, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2048.88, 3),
                (1080, N'10000-S-007', 2, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 435, 3),
                (1081, N'10000-S-008', 2, 1, @item, NULL, 1, NULL, 'GBP', GETUTCDATE(), 75, 3),
      --per migration
                (1082, N'10000-S-069', 2, 1, @migration, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1500, 3),
                (1083, N'10052-S-008', 2, 1, @migration, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2927.93, 3),
                (1084, N'10052-S-013', 2, 1, @migration, NULL, 1, NULL, 'GBP', GETUTCDATE(), 4826.88, 3),
      --per practice
                (1085, N'10004-S-004', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 1300, 3),
                (1086, N'10007-S-003', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 400, 3),
                (1087, N'10047-S-001', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2300, 3),
                (1110, N'10072-S-005', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 150, 3),
                (1111, N'10072-S-010', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 400, 3),
                (1112, N'10072-S-011', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 600, 3),
                (1113, N'10072-S-012', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 200, 3),
                (1114, N'10072-S-013', 2, 1, @practice, NULL, 1, NULL, 'GBP', GETUTCDATE(), 500, 3),
      --per practice merge/split
                (1088, N'10000-S-040', 2, 1, @practiceMergerSplit, NULL, 3, 2, 'GBP', GETUTCDATE(), 1500, 3),
                (1115, N'10000-S-042', 2, 1, @unitMerge, NULL, 3, 2, 'GBP', GETUTCDATE(), 1500, 3),
                (1116, N'10000-S-043', 2, 1, @unitSplit, NULL, 3, 2, 'GBP', GETUTCDATE(), 1500, 3),
      --per seminar
                (1089, N'10004-S-003', 2, 1, @seminar, NULL, 1, NULL, 'GBP', GETUTCDATE(), 850, 3),
      --per training environment
                (1090, N'10052-S-005', 2, 1, @trainingEnvironment, NULL, 1, NULL, 'GBP', GETUTCDATE(), 953.03, 3),
      --per unit merge
                (1091, N'10052-S-009', 2, 1, @unitMerge, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2089.16, 3),
      --per unit split
                (1092, N'10052-S-010', 2, 1, @unitSplit, NULL, 1, NULL, 'GBP', GETUTCDATE(), 2089.16, 3),
      --per user
                (1093, N'10046-S-008', 2, 1, @user, NULL, 1, NULL, 'GBP', GETUTCDATE(), 40, 3);

    DECLARE @InsertedPriceIds TABLE(
        Id INT,
        Price DECIMAL(18,4),
        CataloguePriceTypeId INT
     );

    SET IDENTITY_INSERT catalogue.CataloguePrices ON;

    MERGE INTO catalogue.CataloguePrices AS TARGET
    USING @CataloguePrice AS SOURCE
    ON TARGET.CataloguePriceId = SOURCE.CataloguePriceId
    WHEN MATCHED THEN
           UPDATE SET TARGET.CatalogueItemId = SOURCE.CatalogueItemId,
                      TARGET.ProvisioningTypeId = SOURCE.ProvisioningTypeId,
                      TARGET.CataloguePriceTypeId = SOURCE.CataloguePriceTypeId,
                      TARGET.PricingUnitId = SOURCE.PricingUnitId,
                      TARGET.CurrencyCode = SOURCE.CurrencyCode,
                      TARGET.LastUpdated = SOURCE.LastUpdated,
                      TARGET.PublishedStatusId = SOURCE.PublishedStatusId,
                      TARGET.CataloguePriceCalculationTypeId = SOURCE.CataloguePriceCalculationTypeId,
                      TARGET.CataloguePriceQuantityCalculationTypeId = SOURCE.CataloguePriceQuantityCalculationTypeId
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (CataloguePriceId, CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, PublishedStatusId)
        VALUES (SOURCE.CataloguePriceId, SOURCE.CatalogueItemId, SOURCE.ProvisioningTypeId, SOURCE.CataloguePriceTypeId, SOURCE.PricingUnitId, SOURCE.TimeUnitId, SOURCE.CataloguePriceCalculationTypeId, SOURCE.CataloguePriceQuantityCalculationTypeId, SOURCE.CurrencyCode, SOURCE.LastUpdated, SOURCE.PublishedStatusId)
    OUTPUT INSERTED.CataloguePriceId, SOURCE.Price, INSERTED.CataloguePriceTypeId INTO @InsertedPriceIds (Id, Price, CataloguePriceTypeId);

    --Insert flat Prices
    INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, LowerRange, UpperRange, Price)
    SELECT
        IPI.Id, 1, NULL, IPI.Price
    FROM @InsertedPriceIds IPI
    WHERE CataloguePriceTypeId = 1;

    --Insert Tiered Prices
    INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, LowerRange, UpperRange, Price)
        SELECT
            IPI.Id AS CataloguePriceId,
            1 AS LowerRange,
            9,
            IPI.Price
        FROM @InsertedPriceIds IPI
        WHERE CataloguePriceTypeId = 2
    UNION ALL
        SELECT
            IPI.Id AS CataloguePriceId,
            10 AS LowerRange,
            99,
            IPI.Price / 2
        FROM @InsertedPriceIds IPI
        WHERE CataloguePriceTypeId = 2
    UNION ALL
        SELECT
            IPI.Id AS CataloguePriceId, 
            100 AS LowerRange,
            NULL,
            IPI.Price / 4
        FROM @InsertedPriceIds IPI
        WHERE CataloguePriceTypeId = 2
    ORDER BY
        CataloguePriceId, LowerRange;


    SET IDENTITY_INSERT catalogue.CataloguePrices OFF;
END;
GO
