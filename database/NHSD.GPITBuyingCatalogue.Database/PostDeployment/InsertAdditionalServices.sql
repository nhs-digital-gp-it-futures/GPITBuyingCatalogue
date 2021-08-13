DECLARE @cataloguePriceId AS int = 0;
DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @now AS datetime = GETUTCDATE();
DECLARE @additionalServiceItemType AS int = 2;
DECLARE @solutionId AS nvarchar(14);
DECLARE @additionalServiceId AS nvarchar(14);
DECLARE @additionalServiceId2 AS nvarchar(14);
DECLARE @noUser AS int = NULL;

DECLARE @patient AS smallint = -1;
DECLARE @bed AS smallint = -2;
DECLARE @consultation AS smallint = -3;
DECLARE @licence AS smallint = -4;

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
                 VALUES (@additionalServiceId,'Addition to Write on Time', 'Write on time Addttion Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, @patient, 2, 'GBP', @now, 199.99);

            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 VALUES (@additionalServiceId, 1, 1, @now, @noUser),
                        (@additionalServiceId, 5, 1, @now, @noUser);
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
                 VALUES (@additionalServiceId,'Addition to Appointment Gateway', 'Appointment Gateway Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, @bed, 2, 'GBP', @now, 299.99);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Zen Guidance', 'Zen Guidance Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, @bed, 2, 'GBP', @now, 399.99),
                        (@additionalServiceId2, 2, 1, @consultation, 2, 'GBP', @now, 389.99);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Diagnostics XYZ', 'Diagnostics XYZ Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 2, @bed, 2, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 999, 123.45),
                        (@cataloguePriceId, 1000, 1999, 49.99),
                        (@cataloguePriceId, 2000, NULL, 19.99);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Document Wizard', 'Document Wizard Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 2, @patient, 1, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO catalogue.CataloguePriceTiers(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 9, 100.45),
                        (@cataloguePriceId, 10, 199, 200.99),
                        (@cataloguePriceId, 200, NULL,300.99);
                        
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Paperlite', 'Paperlite Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 3, 1, @licence, null, 'GBP', @now, 499.99);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Medsort', 'Medsort Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, @patient, 2, 'GBP', @now, 599.99);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to Boston Dynamics', 'Boston Dynamics Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, @patient, 2, 'GBP', @now, 599.99);
                 
            INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
                 SELECT @additionalServiceId, Id, 1, @now, @noUser
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
                 VALUES (@additionalServiceId,'Addition to NotEmis Web GP', 'NotEmis Web GP Addition Full Description', @now , @noUser, @solutionId);

            INSERT INTO catalogue.CataloguePrices(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, @bed, 1, 'GBP', @now, 699.99);
        END;
    END;
END;
GO
