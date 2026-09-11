IF (UPPER('$(INSERT_TEST_DATA)') = 'TRUE')
BEGIN
    -- get sue from the db
    DECLARE @sueId INT,
    @OrderingParty INT;
    SELECT
        @sueId = Id,
        @OrderingParty = PrimaryOrganisationId
    FROM
        users.AspNetUsers
    WHERE
        UserName = 'SueSmith@email.com';

    Declare @OrderingPartyOdsCode NVARCHAR(10);

    SELECT
        @OrderingPartyOdsCode = ExternalIdentifier
    FROM
        organisations.Organisations
    WHERE
        Id = @OrderingParty

    DECLARE
        @SupplierId INT = 99999, --notEmis Health,
        @CatalogueSolutionId NVARCHAR(14) = '99999-89', --NotEmis Web GP
        @AdditionalServiceId NVARCHAR(14) = '99999-89-A01', --NotEmis Web GP additional service
        @AssociatedServicesOnly INT = 0,
        @LastBuyerContactId INT,
        @LastSupplierContactId INT;

    DECLARE @CatalogueSolutionPriceId INT = (SELECT TOP 1 CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @CatalogueSolutionId AND PublishedStatusId = 3); --NotEmis Web GP Price
    DECLARE @AdditionalServicePriceId INT = (SELECT TOP 1 CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @AdditionalServiceId AND PublishedStatusId = 3); --NotEmis Web GP additional service Price
    DECLARE @SelectedFrameworkId NVARCHAR(10) = (SELECT Id FROM catalogue.Frameworks WHERE Id = 'TIF001'); --Technology Innovation Framework

    DECLARE @TestOrdersContacts TABLE(
        Id INT NOT NULL,
        FirstName NVARCHAR(100) NULL,
        LastName NVARCHAR(100) NULL,
        Email NVARCHAR(256) NULL,
        Phone NVARCHAR(35) NULL,
        LastUpdated DATETIME2(7) NOT NULL,
        LastUpdatedBy INT NOT NULL,
        SupplierContactId INT NULL);

    -------------------------------------------------------
    -- Insert Sue and Supplier Contacts
    -------------------------------------------------------

    INSERT INTO @TestOrdersContacts
    VALUES
    (
        1, --Buyer Contact
        'Sue',
        'Smith',
        'SueSmith@email.com',
        '1234567',
        SYSDATETIME(),
        @sueId,
        NULL
    ),
    (
        2, -- Supplier Contact
        'notEmis',
        'Health',
        'notEmisHealth@email.com',
        '1234567',
        SYSDATETIME(),
        @sueId,
        NULL
    );

    -------------------------------------------------------
    -- Insert Orders
    -------------------------------------------------------


    -------------------------------------------------------
    --Order with description
    -------------------------------------------------------

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, Created, LastUpdated, LastUpdatedBy, IsDeleted, AssociatedServicesOnly, OrderTypeId)
    VALUES
    (
    1,
    1,
    'Order with description',
    @OrderingParty,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    0,
    @AssociatedServicesOnly,
    1);

    -------------------------------------------------------
    --Order with description and ordering party contact
    -------------------------------------------------------

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1 -- buyer contact id

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, Created, LastUpdated, LastUpdatedBy, IsDeleted, AssociatedServicesOnly, OrderTypeId)
    VALUES
    (
    2,
    1,
    'Order with description and ordering party contact',
    @OrderingParty,
    @LastBuyerContactId,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    0,
    @AssociatedServicesOnly, 
    1);

    -------------------------------------------------------
    --Order with description, ordering party contact and supplier with it's contact
    -------------------------------------------------------

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1 -- buyer contact id

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 2 -- supplier contact id

    SELECT @LastSupplierContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId,
        Created, LastUpdated, LastUpdatedBy, IsDeleted, AssociatedServicesOnly, OrderTypeId)
    VALUES
    (
    3,
    1,
    'Order with description, ordering party contact and supplier with contact',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    0,
    @AssociatedServicesOnly, 
    1);

    -------------------------------------------------------
    --Order with description, ordering party contact, supplier with it's contact and timescales
    -------------------------------------------------------

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1 -- buyer contact id

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 2 -- supplier contact id

    SELECT @LastSupplierContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, CommencementDate,
        Created, LastUpdated, LastUpdatedBy, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly, OrderTypeId)
    VALUES
    (
    4,
    1,
    'Order with description, ordering party contact and supplier with contact and timescales',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    DATEADD(day, 1, SYSDATETIME()),
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    0,
    6,
    36,
    @AssociatedServicesOnly, 
    1);

    -------------------------------------------------------
    --Expired order
    -------------------------------------------------------

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 2

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1

    SELECT @LastSupplierContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, CommencementDate, Created, LastUpdated, LastUpdatedBy, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly, OrderTypeId)
    VALUES
    (7, 1, 'Expired order', @OrderingParty, @LastBuyerContactId, @SupplierId, @LastSupplierContactId, DATEADD(day, -120, SYSDATETIME()), SYSDATETIME(), SYSDATETIME(), @sueId, 0, 1, 3, @AssociatedServicesOnly, 1);

    -------------------------------------------------------
    -- catalogue solution only order completed 
    -------------------------------------------------------

    DECLARE @OrderIdCatSolAdditional TABLE(
        Id INT
    );

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1 -- buyer contact id

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 2 -- supplier contact id

    SELECT @LastSupplierContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, CommencementDate,
        Created, LastUpdated, LastUpdatedBy, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly, OrderTypeId)
        OUTPUT INSERTED.Id INTO @OrderIdCatSolAdditional (Id)
    VALUES
    (
    5,
    1,
    'catalogue solution only order completed',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    DATEADD(day, 1, SYSDATETIME()),
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    0,
    6,
    36,
    @AssociatedServicesOnly, 
    1);

    DECLARE @OrderId INT;
    SELECT @OrderId = Id FROM @OrderIdCatSolAdditional

    --insert catalogue solution and all the steps to complete the order

    UPDATE ordering.Orders
    SET SolutionId = @CatalogueSolutionId,
        DeliveryDate = DATEADD(day, 20, SYSDATETIME())
    WHERE Id = @OrderId;

    DECLARE @OrderItemIdTable TABLE (Id INT);
    DECLARE @OrderItemPriceIdTable TABLE (Id INT);
    DECLARE @OrderItemId INT, @OrderItemPriceId INT;

    INSERT INTO ordering.OrderItemsV2 (OrderId, CatalogueItemId, ParentId, EstimationPeriodId, Created, LastUpdated, LastUpdatedBy)
    OUTPUT INSERTED.Id INTO @OrderItemIdTable (Id)
    VALUES (@OrderId, @CatalogueSolutionId, NULL, 1, SYSDATETIME(), SYSDATETIME(), @sueId);

    SELECT @OrderItemId = Id FROM @OrderItemIdTable;

    INSERT INTO ordering.OrderItemPricesV2
        (OrderItemId, CataloguePriceId, BillingPeriodId, ProvisioningTypeId,
         CataloguePriceTypeId, CataloguePriceCalculationTypeId, CataloguePriceQuantityCalculationTypeId,
         CurrencyCode, Description, RangeDescription, LastUpdated, LastUpdatedBy)
    OUTPUT INSERTED.Id INTO @OrderItemPriceIdTable (Id)
    SELECT
        @OrderItemId,
        CP.CataloguePriceId,
        CP.TimeUnitId,
        CP.ProvisioningTypeId,
        CP.CataloguePriceTypeId,
        CP.CataloguePriceCalculationTypeId,
        NULL,
        CP.CurrencyCode,
        PU.Description,
        PU.RangeDescription,
        SYSDATETIME(),
        @sueId
    FROM catalogue.CataloguePrices CP
    INNER JOIN catalogue.PricingUnits PU
        ON CP.PricingUnitId = PU.Id
    WHERE CataloguePriceId = @CatalogueSolutionPriceId

    SELECT @OrderItemPriceId = Id FROM @OrderItemPriceIdTable;

    INSERT INTO ordering.OrderItemPriceTiers (OrderItemPriceId, Price, ListPrice, LowerRange, UpperRange, LastUpdated, LastUpdatedBy)
    SELECT
        @OrderItemPriceId,
        Price,
        Price,
        LowerRange,
        UpperRange,
        SYSDATETIME(),
        @sueId
    FROM catalogue.CataloguePriceTiers
    WHERE CataloguePriceId = @CatalogueSolutionPriceId

    INSERT INTO ordering.OrderSublocations (OrderId, SublocationOdsCode, OwnerOdsCode)
    VALUES
    (@OrderId, '02T', @OrderingPartyOdsCode)

    INSERT INTO ordering.OrderSublocationRecipients (OrderId, ParentSublocationOdsCode, RecipientOdsCode)
    VALUES    
    (@OrderId, '02T', 'B84016'),
    (@OrderId, '02T', 'B84613');

    INSERT INTO ordering.OrderItemSublocationRecipientsV2 (OrderItemId, OrderId, ParentSublocationOdsCode, RecipientOdsCode, Quantity, DeliveryDate, LastUpdated, LastUpdatedBy)
    VALUES
    (@OrderItemId, @OrderId, '02T', 'B84016', 200, DATEADD(day, 20, SYSDATETIME()), SYSDATETIME(), @sueId),
    (@OrderItemId, @OrderId, '02T', 'B84613', 200, DATEADD(day, 20, SYSDATETIME()), SYSDATETIME(), @sueId);

    -- Funding source step

    UPDATE ordering.Orders
    SET SelectedFrameworkId = @SelectedFrameworkId
    WHERE Id = @OrderId;

    INSERT INTO ordering.OrderItemFundingV2 (OrderItemId, OrderItemFundingType, LastUpdated, LastUpdatedBy)
    VALUES (@OrderItemId, 2, SYSDATETIME(), @sueId);

    -- Implementation milestone step

    DECLARE @ContractId INT;

    IF EXISTS (SELECT 1 FROM ordering.Contracts WHERE OrderId = @OrderId)
    BEGIN
        SELECT @ContractId = Id FROM ordering.Contracts WHERE OrderId = @OrderId;
    END
    ELSE
    BEGIN
        DECLARE @ContractIdTable TABLE (Id INT);
        INSERT INTO ordering.Contracts (OrderId)
        OUTPUT INSERTED.Id INTO @ContractIdTable (Id)
        VALUES (@OrderId);
        SELECT @ContractId = Id FROM @ContractIdTable;
    END

    INSERT INTO ordering.ImplementationPlans (ContractId, IsDefault, LastUpdated, LastUpdatedBy)
    VALUES (@ContractId, 0, SYSDATETIME(), @sueId);

    -- Data processing information step

    IF NOT EXISTS (SELECT 1 FROM ordering.ContractFlags WHERE OrderId = @OrderId)
    BEGIN
        INSERT INTO ordering.ContractFlags (OrderId, UseDefaultDataProcessing, LastUpdated, LastUpdatedBy)
        VALUES (@OrderId, 1, SYSDATETIME(), @sueId);
    END

    -- Declaration step

    UPDATE ordering.Orders
    SET AcceptedTermsAndConditions = 1
    WHERE Id = @OrderId;

    -- Review and complete order step

    UPDATE ordering.Orders
    SET Completed = SYSDATETIME()
    WHERE Id = @OrderId;

    --insert add ser (left on the old tables for now - additional service data needs fixing separately)

    INSERT INTO ordering.OrderItems (OrderId, CatalogueItemId, Created, LastUpdated)
    VALUES(@orderId, @AdditionalServiceId, SYSDATETIME(), SYSDATETIME());

    INSERT INTO ordering.OrderItemPrices (OrderId, CatalogueItemId, CataloguePriceId, BillingPeriodId, ProvisioningTypeId,
        CataloguePriceTypeId, CataloguePriceCalculationTypeId, CurrencyCode, Description, RangeDescription)
    SELECT
        @orderId,
        @AdditionalServiceId,
        CP.CataloguePriceId,
        CP.TimeUnitId,
        CP.ProvisioningTypeId,
        CP.CataloguePriceTypeId,
        CP.CataloguePriceCalculationTypeId,
        CP.CurrencyCode,
        PU.Description,
        PU.RangeDescription
    FROM catalogue.CataloguePrices CP
    INNER JOIN catalogue.PricingUnits PU
	    ON CP.PricingUnitId = PU.Id
    WHERE CataloguePriceId = @AdditionalServicePriceId

    INSERT INTO ordering.OrderItemPriceTiers (OrderId, CatalogueItemId, Price, ListPrice, LowerRange, UpperRange)
    SELECT
        @OrderId,
        @AdditionalServiceId,
        Price,
        Price,
        LowerRange,
        UpperRange
    FROM catalogue.CataloguePriceTiers
    WHERE CataloguePriceId = @AdditionalServicePriceId

    INSERT INTO ordering.OrderItemSublocationRecipients (OrderId, CatalogueItemId, ParentSublocationOdsCode, RecipientOdsCode, Quantity)
    VALUES
    (@OrderId, @AdditionalServiceId, '02T', 'B84007', 123),
    (@OrderId, @AdditionalServiceId, '02T', 'B84016', 234),
    (@OrderId, @AdditionalServiceId, '02T', 'B84613', 345),
    (@OrderId, @AdditionalServiceId, '02T', 'Y02572', 456);

    UPDATE ordering.Orders SET OrderNumber = Id
END
