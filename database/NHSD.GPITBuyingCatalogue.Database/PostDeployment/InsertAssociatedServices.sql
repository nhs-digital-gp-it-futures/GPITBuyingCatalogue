DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @declarative AS int = (SELECT ProvisioningTypeId FROM dbo.ProvisioningType WHERE [Name] = 'Declarative');
DECLARE @onDemand AS int = (SELECT ProvisioningTypeId FROM dbo.ProvisioningType WHERE [Name] = 'OnDemand');

DECLARE @flat AS int = (SELECT CataloguePriceTypeId FROM dbo.CataloguePriceType WHERE [Name] = 'Flat');
DECLARE @tiered AS int = (SELECT CataloguePriceTypeId FROM dbo.CataloguePriceType WHERE [Name] = 'Tiered');

DECLARE @hour AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'hour');
DECLARE @course AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'course');
DECLARE @halfDay AS uniqueidentifier = (SELECT PricingUnitId FROM dbo.PricingUnit WHERE [Name] = 'halfDay');

DECLARE @associatedServiceItemType AS int = (SELECT CatalogueItemTypeId FROM dbo.CatalogueItemType WHERE [Name] = 'Associated Service');
DECLARE @publishedStatus AS int = (SELECT Id FROM dbo.PublicationStatus WHERE [Name] = 'Published');

DECLARE @gbp AS char(3) = 'GBP';

DECLARE @associatedServiceId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM dbo.AssociatedService)
BEGIN
    SET @associatedServiceId = '100000-S-001';

    INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, 'Really Kool associated service', '100000', @publishedStatus, @now);

    INSERT INTO dbo.AssociatedService (AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 'Really Kool associated service', NULL, @now, @emptyGuid);

    INSERT INTO dbo.CataloguePrice (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @declarative, @flat, @course, NULL, @gbp, @now, 99.99),
                (@associatedServiceId, @onDemand, @flat, @halfDay, NULL, @gbp, @now, 150.00);

    SET @associatedServiceId = '100000-S-002';

    INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, 'Really Kool tiered associated service', '100000', @publishedStatus, @now);

    INSERT INTO dbo.AssociatedService (AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 'Really Kool tiered associated service', NULL, @now, @emptyGuid);

    INSERT INTO dbo.CataloguePrice (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @tiered, @hour, NULL, @gbp, @now, NULL);

    DECLARE @tieredPriceId AS int = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @associatedServiceId AND CataloguePriceTypeId = @tiered);

    INSERT INTO dbo.CataloguePriceTier (CataloguePriceId, BandStart, BandEnd, Price)
         VALUES (@tieredPriceId, 1, 9, 100),
                (@tieredPriceId, 10, NULL, 49.99);
END;
GO
