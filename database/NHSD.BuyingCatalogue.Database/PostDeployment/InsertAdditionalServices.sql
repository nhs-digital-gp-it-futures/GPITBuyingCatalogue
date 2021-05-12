DECLARE @cataloguePriceId AS int = 0;
DECLARE @publishedStatus AS int = 3;
DECLARE @solutionItemType AS int = 1;
DECLARE @now AS datetime = GETUTCDATE();
DECLARE @additionalServiceItemType AS int = 2;
DECLARE @solutionId AS nvarchar(14);
DECLARE @additionalServiceId AS nvarchar(14);
DECLARE @additionalServiceId2 AS nvarchar(14);
DECLARE @emptyGuid AS uniqueidentifier = '00000000-0000-0000-0000-000000000000';

IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND EXISTS (SELECT * FROM dbo.Solution)
BEGIN
    SET @solutionId = '100000-001';
    SET @additionalServiceId = '100000-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Write on Time additional service', '100000', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Write on Time', 'Write on time Addttion Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 199.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100001-001';
    SET @additionalServiceId = '100001-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Appointment Gateway additional service', '100001', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Appointment Gateway', 'Appointment Gateway Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 299.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100002-001';
    SET @additionalServiceId = '100002-001-A01'
    SET @additionalServiceId2 = '100002-001-A02'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Zen Guidance additional service', '100002', @publishedStatus, @now),
                        (@additionalServiceId2, @additionalServiceItemType, 'Zen Guidance additional service 2', '100002', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Zen Guidance', 'Zen Guidance Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 399.99),
                        (@additionalServiceId2, 2, 1, '774E5A1D-D15C-4A37-9990-81861BEAE42B', 2, 'GBP', @now, 389.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100004-001';
    SET @additionalServiceId = '100004-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Diagnostics XYZ additional service', '100004', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Diagnostics XYZ', 'Diagnostics XYZ Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 2, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 2, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 999, 123.45),
                        (@cataloguePriceId, 1000, 1999, 49.99),
                        (@cataloguePriceId, 2000, NULL, 19.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100005-001';
    SET @additionalServiceId = '100005-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Document Wizard additional service', '100005', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Document Wizard', 'Document Wizard Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 2, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 1, 'GBP', @now, 499.99);

            SET @cataloguePriceId = (SELECT CataloguePriceId FROM dbo.CataloguePrice WHERE CatalogueItemId = @additionalServiceId);

            INSERT INTO dbo.CataloguePriceTier(CataloguePriceId, BandStart, BandEnd, Price)
                 VALUES (@cataloguePriceId, 1, 9, 100.45),
                        (@cataloguePriceId, 10, 199, 200.99),
                        (@cataloguePriceId, 200, NULL,300.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100006-001';
    SET @additionalServiceId = '100006-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Paperlite additional service', '100006', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Paperlite', 'Paperlite Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 3, 1, '8BF9C2F9-2FD7-4A29-8406-3C6B7B2E5D65', null, 'GBP', @now, 499.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-001';
    SET @additionalServiceId = '100007-001-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Medsort additional service', '100007', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Medsort', 'Medsort Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 599.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '100007-002';
    SET @additionalServiceId = '100007-002-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'Boston Dynamics additional service', '100007', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to Boston Dynamics', 'Boston Dynamics Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 1, 1, 'F8D06518-1A20-4FBA-B369-AB583F9FA8C0', 2, 'GBP', @now, 599.99);
        END;
    END;

    /***************************************************************************************************************************/

    SET @solutionId = '99999-89';
    SET @additionalServiceId = '99999-89-A01'

    IF EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @solutionId)
    BEGIN
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @additionalServiceId)
        BEGIN
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, CatalogueItemTypeId, [Name], SupplierId, PublishedStatusId, Created)
                 VALUES (@additionalServiceId, @additionalServiceItemType, 'NotEmis Web GP additional service', '99999', @publishedStatus, @now);

            INSERT INTO dbo.AdditionalService(CatalogueItemId, Summary, FullDescription, LastUpdated, LastUpdatedBy, SolutionId)
                 VALUES (@additionalServiceId,'Addition to NotEmis Web GP', 'NotEmis Web GP Addition Full Description', @now , @emptyGuid, @solutionId);

            INSERT INTO dbo.CataloguePrice(CatalogueItemId, ProvisioningTypeId, CataloguePriceTypeId, PricingUnitId, TimeUnitId, CurrencyCode, LastUpdated, Price) 
                 VALUES (@additionalServiceId, 2, 1, 'D43C661A-0587-45E1-B315-5E5091D6E9D0', 1, 'GBP', @now, 699.99);
        END;
    END;
END;
GO
