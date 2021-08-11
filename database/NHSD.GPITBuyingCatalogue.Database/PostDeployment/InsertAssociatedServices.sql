DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @now AS datetime = GETUTCDATE();

DECLARE @declarative AS int = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'Declarative');
DECLARE @onDemand AS int = (SELECT Id FROM catalogue.ProvisioningTypes WHERE [Name] = 'OnDemand');

DECLARE @flat AS int = (SELECT Id FROM catalogue.CataloguePriceTypes WHERE [Name] = 'Flat');
DECLARE @tiered AS int = (SELECT Id FROM catalogue.CataloguePriceTypes WHERE [Name] = 'Tiered');

DECLARE @hour AS uniqueidentifier = (SELECT Id FROM catalogue.PricingUnits WHERE [Name] = 'hour');
DECLARE @course AS uniqueidentifier = (SELECT Id FROM catalogue.PricingUnits WHERE [Name] = 'course');
DECLARE @halfDay AS uniqueidentifier = (SELECT Id FROM catalogue.PricingUnits WHERE [Name] = 'halfDay');

DECLARE @associatedServiceItemType AS int = (SELECT Id FROM catalogue.CatalogueItemTypes WHERE [Name] = 'Associated Service');
DECLARE @publishedStatus AS int = (SELECT Id FROM catalogue.PublicationStatus WHERE [Name] = 'Published');

DECLARE @gbp AS char(3) = 'GBP';

DECLARE @associatedServiceId AS nvarchar(14);

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM catalogue.AssociatedServices)
BEGIN
    SET @associatedServiceId = '100000-S-001';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Practice reorganisation', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 
                N'Our practice reorganisation service provides support during practice splits and mergers. When two or more practices decide to split or merge clinical information, our service can target either select patients or the full patient list. It makes organising a change in practice simple. This service is applicable to both implementation and live operations.', 
                N'The typical unit of order for practice mergers and splits is likely to be on an individual basis (one). This is a fixed fee per practice regardless of size.',
                @now, @emptyGuid);

    INSERT INTO catalogue.CataloguePrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @declarative, @flat, @course, NULL, @gbp, @now, 99.99),
                (@associatedServiceId, @onDemand, @flat, @halfDay, NULL, @gbp, @now, 150.00);

    SET @associatedServiceId = '100000-S-002';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Engineering', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 
                N'This service ensure practice infrastructure is in place and has optimum configuration. This service is applicable to the implementation phase and live operations.', 
                N'Dependent on the issue, a typical range is 1-7 days. Additional Engineering is available and would be dependant on scale and user needs. This could be one day or purchased in bulk.',
                @now, @emptyGuid);

    INSERT INTO catalogue.CataloguePrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @tiered, @hour, NULL, @gbp, @now, 595);

    DECLARE @tieredPriceId AS int = (SELECT CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @associatedServiceId AND CataloguePriceTypeId = @tiered);

    INSERT INTO catalogue.CataloguePriceTiers (CataloguePriceId, BandStart, BandEnd, Price)
         VALUES (@tieredPriceId, 1, 9, 100),
                (@tieredPriceId, 10, NULL, 49.99);

     SET @associatedServiceId = '100000-S-003';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Lloyd George digitisation', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 
                 N'A fully managed, ISO compliant, service for the digitisation of Lloyd George records. We’ll collect your paper records, scan them and upload them into EMIS Web as digital files. Following sign-off paper records are securely destroyed by shredding them and then recycling them in an eco-friendly way.', 
                 N'The typical volume would be an average of eight thousand which is the typical practice capitation. The largest practice size is the maximum.',
                @now, @emptyGuid);

    INSERT INTO catalogue.CataloguePrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @flat, @hour, NULL, @gbp, @now, 4.35);


     SET @associatedServiceId = '100000-S-004';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Lloyd George digitisation (upload only)', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 
                N'We will take digitised Lloyd George files and upload them into EMIS Web. This allows for full visibility of a patients medical records from within the clinical system for more informed decisions at the point of care.', 
                N'The typical volume would be an average of eight thousand which is the typical practice capitation. The largest practice size is the maximum.',
                @now, @emptyGuid);

    INSERT INTO catalogue.CataloguePrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @flat, @hour, NULL, @gbp, @now, 1.25);


     SET @associatedServiceId = '100000-S-005';

    INSERT INTO catalogue.CatalogueItems(Id, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
         VALUES (@associatedServiceId, @associatedServiceItemType, N'Project management', 100000, @publishedStatus, @now);

    INSERT INTO catalogue.AssociatedServices (CatalogueItemId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy) 
         VALUES (@associatedServiceId, 
                 N'Our project management service can help organisations to seamlessly manage end-to-end projects based on their requirements. From initial planning to roll-out and ongoing project management, our team provides tailored support to customers.', 
                 N'Project Management resource is anticipated to be low for standard installations. For more complex deployments it is anticipated to be higher. The minimum is 1 day, maximum is 10 days per practice',
                @now, @emptyGuid);

    INSERT INTO catalogue.CataloguePrices (CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price)
         VALUES (@associatedServiceId, @onDemand, @flat, @hour, NULL, @gbp, @now, 205);

END;
GO
