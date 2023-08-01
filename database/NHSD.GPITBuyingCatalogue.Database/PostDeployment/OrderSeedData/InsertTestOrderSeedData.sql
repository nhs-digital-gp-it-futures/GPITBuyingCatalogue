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

    DECLARE
        @OrderStatus INT = 2,
        @SupplierId INT = 99999, --notEmis Health,
        @CatalogueSolutionId NVARCHAR(14) = '99999-89', --NotEmis Web GP
        @CatalogueSolutionPriceId INT = 19, --NotEmis Web GP Tiered Price
        @AdditionalServiceId NVARCHAR(14) = '99999-89-A01', --NotEmis Web GP additional service
        @AdditionalServicePriceId INT = 29, --NotEmis Web GP Additional Service Flat Price
        @AssociatedServicesOnly INT = 0,
        @LastBuyerContactId INT,
        @LastSupplierContactId INT;

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
    (OrderNumber, Revision, Description, OrderingPartyId, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
    VALUES
    (
    1,
    1,
    'Order with description',
    @OrderingParty,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

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
    (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
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
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

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
        Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
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
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

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
        Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly)
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
    @OrderStatus,
    0,
    6,
    36,
    @AssociatedServicesOnly);

    -------------------------------------------------------
    -- order with catalogue solution and additional service
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
        Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly)
        OUTPUT INSERTED.Id INTO @OrderIdCatSolAdditional (Id)
    VALUES
    (
    5,
    1,
    'Order with catalogue solution and additional service',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    DATEADD(day, 1, SYSDATETIME()),
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    6,
    36,
    @AssociatedServicesOnly);

    DECLARE @OrderId INT;
    SELECT @OrderId = Id FROM @OrderIdCatSolAdditional

    --insert cat sol

    INSERT INTO ordering.OrderItems (OrderId, CatalogueItemId, Created, LastUpdated)
    VALUES(@orderId, @CatalogueSolutionId, SYSDATETIME(), SYSDATETIME())

    INSERT INTO ordering.OrderItemPrices (OrderId, CatalogueItemId, CataloguePriceId, BillingPeriodId, ProvisioningTypeId,
        CataloguePriceTypeId, CataloguePriceCalculationTypeId, CurrencyCode, Description, RangeDescription)
    SELECT
        @orderId,
        @CatalogueSolutionId,
        0,
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
    WHERE CataloguePriceId = @CatalogueSolutionPriceId

    INSERT INTO ordering.OrderItemPriceTiers (OrderId, CatalogueItemId, Price, ListPrice, LowerRange, UpperRange)
    SELECT
        @OrderId,
        @CatalogueSolutionId,
        Price,
        Price,
        LowerRange,
        UpperRange
    FROM catalogue.CataloguePriceTiers
    WHERE CataloguePriceId = @CatalogueSolutionPriceId


    INSERT INTO ordering.OrderItemRecipients (OrderId, CatalogueItemId, OdsCode, Quantity)
    VALUES
    (@OrderId, @CatalogueSolutionId, 'B84007', 123),
    (@OrderId, @CatalogueSolutionId, 'B84016', 234),
    (@OrderId, @CatalogueSolutionId, 'B84613', 345),
    (@OrderId, @CatalogueSolutionId, 'Y02572', 456);

    --insert add ser

    INSERT INTO ordering.OrderItems (OrderId, CatalogueItemId, Created, LastUpdated)
    VALUES(@orderId, @AdditionalServiceId, SYSDATETIME(), SYSDATETIME());

    INSERT INTO ordering.OrderItemPrices (OrderId, CatalogueItemId, CataloguePriceId, BillingPeriodId, ProvisioningTypeId,
        CataloguePriceTypeId, CataloguePriceCalculationTypeId, CurrencyCode, Description, RangeDescription)
    SELECT
        @orderId,
        @AdditionalServiceId,
        0,
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

    INSERT INTO ordering.OrderItemRecipients (OrderId, CatalogueItemId, OdsCode, Quantity)
    VALUES
    (@OrderId, @AdditionalServiceId, 'B84007', 123),
    (@OrderId, @AdditionalServiceId, 'B84016', 234),
    (@OrderId, @AdditionalServiceId, 'B84613', 345),
    (@OrderId, @AdditionalServiceId, 'Y02572', 456);

    UPDATE ordering.Orders SET OrderNumber = Id
END
