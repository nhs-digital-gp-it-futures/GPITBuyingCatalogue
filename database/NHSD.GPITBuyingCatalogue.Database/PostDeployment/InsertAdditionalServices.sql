﻿DECLARE @cataloguePriceId AS int = 0;
DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @now AS datetime = GETUTCDATE();
DECLARE @additionalServiceItemType AS int = 2;
DECLARE @solutionId AS nvarchar(14);
DECLARE @additionalServiceId AS nvarchar(14);
DECLARE @additionalServiceId2 AS nvarchar(14);
DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND EXISTS (SELECT * FROM catalogue.Solutions)
BEGIN
    SET @solutionId = '100000-001';
    SET @additionalServiceId = '100000-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Write on Time additional service', 100000, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Write on Time', 'Write on time Addttion Full Description', @now , @bobUser, @solutionId);

            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 VALUES (@additionalServiceId, 1, 1, @now, @bobUser),
                        (@additionalServiceId, 5, 1, @now, @bobUser);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100001-001';
    SET @additionalServiceId = '100001-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Appointment Gateway additional service', 100001, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Appointment Gateway', 'Appointment Gateway Addition Full Description', @now , @bobUser, @solutionId);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C1', 'C5');
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100002-001';
    SET @additionalServiceId = '100002-001-A01'
    SET @additionalServiceId2 = '100002-001-A02'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Zen Guidance additional service', 100002, @publishedStatus, @now),
                        (@additionalServiceId2, @additionalServiceItemType, 'Zen Guidance additional service 2', 100002, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Zen Guidance', 'Zen Guidance Addition Full Description', @now , @bobUser, @solutionId);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C6';
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100004-001';
    SET @additionalServiceId = '100004-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Diagnostics XYZ additional service', 100004, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Diagnostics XYZ', 'Diagnostics XYZ Addition Full Description', @now , @bobUser, @solutionId);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef IN ('C7', 'C15');
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100005-001';
    SET @additionalServiceId = '100005-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Document Wizard additional service', 100005, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Document Wizard', 'Document Wizard Addition Full Description', @now , @bobUser, @solutionId);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @bobUser
               FROM catalogue.Capabilities
              WHERE CapabilityRef = 'C8';
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100006-001';
    SET @additionalServiceId = '100006-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Paperlite additional service', 100006, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Paperlite', 'Paperlite Addition Full Description', @now , @bobUser, @solutionId);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @bobUser
                   FROM catalogue.Capabilities
                  WHERE CapabilityRef IN ('C9', 'C19', 'C41');
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-001';
    SET @additionalServiceId = '100007-001-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Medsort additional service', 100007, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Medsort', 'Medsort Addition Full Description', @now , @bobUser, @solutionId);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @bobUser
                   FROM catalogue.Capabilities
                  WHERE CapabilityRef IN ('C9', 'C17');
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-002';
    SET @additionalServiceId = '100007-002-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Boston Dynamics additional service', 100007, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Boston Dynamics', 'Boston Dynamics Addition Full Description', @now , @bobUser, @solutionId);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @bobUser
                   FROM catalogue.Capabilities
                  WHERE CapabilityRef = 'C30';
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '99999-89';
    SET @additionalServiceId = '99999-89-A01'

    IF EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM catalogue.CatalogueItems WHERE Id = @additionalServiceId)
        BEGIN
            INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'NotEmis Web GP additional service', 99999, @publishedStatus, @now);

            INSERT INTO catalogue.AdditionalServices(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to NotEmis Web GP', 'NotEmis Web GP Addition Full Description', @now , @bobUser, @solutionId);
        END;
    END;

    /* Insert Prices */

    DECLARE @patient AS smallint = -1;
    DECLARE @bed AS smallint = -2;
    DECLARE @consultation AS smallint = -3;
    DECLARE @licence AS smallint = -4;

    DECLARE @InsertedPriceIds TABLE(
         Id INT,
         Price DECIMAL(18,4),
         CataloguePriceTypeId INT
     );

    DECLARE @AdditionalServiesPrices TABLE(
        CatalogueItemId NVARCHAR(14),
        ProvisioningTypeId INT,
        CataloguePriceTypeId INT,
        PricingUnitId INT,
        TimeUnitId INT,
        CataloguePriceCalculationTypeId INT,
        CurrencyCode NVARCHAR(3),
        LastUpdated DATETIME2(0),
        Price DECIMAL(18,4),
        PublishedStatusId INT
    );

    INSERT INTO @AdditionalServiesPrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CurrencyCode, LastUpdated, Price, PublishedStatusId)
    VALUES
    ('100000-001-A01', 1, 1, @patient, 2, 1, 'GBP', @now, 199.99, 3),
    ('100001-001-A01', 2, 1, @bed, 2, 1, 'GBP', @now, 299.99, 3),
    ('100002-001-A01', 2, 1, @bed, 2, 1, 'GBP', @now, 399.99, 3),
    ('100002-001-A02', 2, 1, @consultation, 2, 1, 'GBP', @now, 389.99, 3),
    ('100004-001-A01', 2, 2, @bed, 2, 1, 'GBP', @now, 499.99, 3),
    ('100005-001-A01', 1, 2, @patient, 1, 1, 'GBP', @now, 499.99, 3),
    ('100006-001-A01', 3, 1, @licence, null, 1, 'GBP', @now, 499.99, 3),
    ('100007-001-A01', 1, 1, @patient, 2, 1, 'GBP', @now, 599.99, 3),
    ('100007-002-A01', 1, 1, @patient, 2, 1, 'GBP', @now, 599.99, 3),
    ('99999-89-A01', 2, 1, @bed, 1, 1, 'GBP', @now, 699.99, 3);

	MERGE INTO catalogue.CataloguePrices USING @AdditionalServiesPrices AS ASP ON 1 = 0
	WHEN NOT MATCHED THEN
	INSERT (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CurrencyCode, LastUpdated, PublishedStatusId)
	VALUES(    
	ASP.CatalogueItemId,
    ASP.ProvisioningTypeId,
    ASP.CataloguePriceTypeId,
    ASP.PricingUnitId,
    ASP.TimeUnitId,
    ASP.CataloguePriceCalculationTypeId,
    ASP.CurrencyCode,
    ASP.LastUpdated,
    ASP.PublishedStatusId)
	OUTPUT INSERTED.CataloguePriceId, ASP.Price, INSERTED.CataloguePriceTypeId
	INTO @InsertedPriceIds (Id, Price, CataloguePriceTypeId);

    --Insert flat Prices
    INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, LowerRange, UpperRange, Price)
    SELECT
        IPI.Id, 1, NULL, IPI.Price
    FROM @InsertedPriceIds IPI
    WHERE CataloguePriceTypeId = 1

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
END;
GO
