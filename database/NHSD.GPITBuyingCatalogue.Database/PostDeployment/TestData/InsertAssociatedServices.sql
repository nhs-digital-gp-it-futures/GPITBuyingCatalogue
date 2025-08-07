DECLARE @bobEmail AS nvarchar(50) = N'BobSmith@email.com';
DECLARE @bobUser AS int = (SELECT Id FROM users.AspNetUsers WHERE Email = @bobEmail);
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @associatedServiceItemType AS int = (SELECT Id FROM catalogue.CatalogueItemTypes WHERE [Name] = 'Associated Service');
DECLARE @publishedStatus AS int = (SELECT Id FROM catalogue.PublicationStatus WHERE [Name] = 'Published');

DECLARE @gbp AS char(3) = 'GBP';

DECLARE @associatedServiceId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM catalogue.AssociatedServices)
BEGIN
    SET @associatedServiceId = '100000-S-001';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Practice Merge', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, PracticeReorganisationType, LastUpdated, LastUpdatedBy)
         VALUES (@associatedServiceId,
                N'This service merges a single system into another. This service will be required by organisations who previously operated independently but are planning to merge their services, and so need to share a single instance of their software solution.\r\n\r\nPurchasing will allow two practices will combine the elements of both units (including patient lists, staff members, caseloads, etc.) into a single system.', 
                N'Dependent on business need. This Associated Service (and corresponding fee) is for two practices merging into one, regardless of practice size. If more than two practices are merging, an additional Practice Merge will need to be purchased for each additional practice.', 
                1,
                @now, @bobUser);

    /***************************************************************************************************************************/

    SET @associatedServiceId = '100000-S-002';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Engineering', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, PracticeReorganisationType, LastUpdated, LastUpdatedBy)
         VALUES (@associatedServiceId,
                N'This service ensure practice infrastructure is in place and has optimum configuration. This service is applicable to the implementation phase and live operations.',
                N'Dependent on the issue, a typical range is 1-7 days. Additional Engineering is available and would be dependant on scale and user needs. This could be one day or purchased in bulk.',
                0,
                @now, @bobUser);

    /***************************************************************************************************************************/

     SET @associatedServiceId = '100000-S-003';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Lloyd George digitisation', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, PracticeReorganisationType, LastUpdated, LastUpdatedBy)
         VALUES (@associatedServiceId,
                 N'A fully managed, ISO compliant, service for the digitisation of Lloyd George records. We’ll collect your paper records, scan them and upload them into EMIS Web as digital files. Following sign-off paper records are securely destroyed by shredding them and then recycling them in an eco-friendly way.',
                 N'The typical volume would be an average of eight thousand which is the typical practice capitation. The largest practice size is the maximum.',
                 0,
                 @now, @bobUser);

    /***************************************************************************************************************************/

     SET @associatedServiceId = '100000-S-004';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Lloyd George digitisation (upload only)', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, PracticeReorganisationType, LastUpdated, LastUpdatedBy)
         VALUES (@associatedServiceId,
                N'We will take digitised Lloyd George files and upload them into EMIS Web. This allows for full visibility of a patients medical records from within the clinical system for more informed decisions at the point of care.',
                N'The typical volume would be an average of eight thousand which is the typical practice capitation. The largest practice size is the maximum.',
                0,
                @now, @bobUser);

    /***************************************************************************************************************************/

     SET @associatedServiceId = '100000-S-005';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Project management', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, PracticeReorganisationType, LastUpdated, LastUpdatedBy)
         VALUES (@associatedServiceId,
                 N'Our project management service can help organisations to seamlessly manage end-to-end projects based on their requirements. From initial planning to roll-out and ongoing project management, our team provides tailored support to customers.',
                 N'Project Management resource is anticipated to be low for standard installations. For more complex deployments it is anticipated to be higher. The minimum is 1 day, maximum is 10 days per practice',
                 0,
                 @now, @bobUser);

    /* Insert Prices */

    DECLARE @declarative INT = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'Declarative');
    DECLARE @onDemand INT = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'OnDemand');
    DECLARE @perServiceRecipient INT = (Select Id FROM catalogue.CataloguePriceQuantityCalculationTypes WHERE [Name] = 'PerServiceRecipient');

    DECLARE @flat INT = (SELECT Id FROM catalogue.CataloguePriceTypes WHERE [Name] = 'Flat');
    DECLARE @tiered INT = (SELECT Id FROM catalogue.CataloguePriceTypes WHERE [Name] = 'Tiered');

    DECLARE @halfDay AS SMALLINT = -6;
    DECLARE @hour AS SMALLINT = -7;
    DECLARE @course AS SMALLINT = -8;

    DECLARE @InsertedPriceIds TABLE(
         Id INT,
         Price DECIMAL(18,4),
         CataloguePriceTypeId INT
     );

    DECLARE @AssociatedServicesPrices TABLE(
        CatalogueItemId NVARCHAR(14),
        ProvisioningTypeId INT,
        CataloguePriceTypeId INT,
        PricingUnitId INT,
        TimeUnitId INT,
        CataloguePriceCalculationTypeId INT,
        CataloguePriceQuantityCalculationTypeId INT,
        CurrencyCode NVARCHAR(3),
        LastUpdated DATETIME2(0),
        Price DECIMAL(18,4),
        PublishedStatusId INT
    );

    INSERT INTO @AssociatedServicesPrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, Price, PublishedStatusId)
    VALUES
    ('100000-S-001', @declarative, @flat, @course, NULL, 3, @perServiceRecipient, @gbp, @now, 99.99, 3),
    ('100000-S-001', @declarative, @flat, @halfDay, NULL, 3, @perServiceRecipient, @gbp, @now, 150.00, 3),
    ('100000-S-002', @onDemand, @tiered, @hour, NULL, 1, NULL, @gbp, @now, 595, 3),
    ('100000-S-003', @onDemand, @flat, @hour, NULL, 1, NULL, @gbp, @now, 4.35, 3),
    ('100000-S-004', @onDemand, @flat, @hour, NULL, 3, @perServiceRecipient, @gbp, @now, 1.25, 3),
    ('100000-S-005', @onDemand, @flat, @hour, NULL, 3, @perServiceRecipient, @gbp, @now, 205, 3);

	MERGE INTO catalogue.CataloguePrices USING @AssociatedServicesPrices AS ASP ON 1 = 0
	WHEN NOT MATCHED THEN
	INSERT (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId, CurrencyCode, LastUpdated, PublishedStatusId)
	VALUES(    
	ASP.CatalogueItemId,
    ASP.ProvisioningTypeId,
    ASP.CataloguePriceTypeId,
    ASP.PricingUnitId,
    ASP.TimeUnitId,
    ASP.CataloguePriceCalculationTypeId,
    ASP.CataloguePriceQuantityCalculationTypeId,
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
