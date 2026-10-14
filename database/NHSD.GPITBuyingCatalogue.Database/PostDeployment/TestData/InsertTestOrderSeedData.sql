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

    -- Emis Health / Emis Web GP
    DECLARE
        @SupplierId INT = 10000, --Emis Health
        @CatalogueSolutionId NVARCHAR(14) = '10000-001', --Emis Web GP
        @AdditionalServiceId NVARCHAR(14) = '10000-001A003', --Automated Arrivals
        @AssociatedServiceId NVARCHAR(14) = '10000-S-002', --Installation
        @AssociatedServicesOnly INT = 0,
        @LastBuyerContactId INT,
        @LastSupplierContactId INT;

    -- Recipient and sublocation ODS codes
    DECLARE
        @SublocationOdsCode NVARCHAR(10) = '02T',
        @RecipientB84016 NVARCHAR(10) = 'B84016',
        @RecipientB84613 NVARCHAR(10) = 'B84613';

    DECLARE @CatalogueSolutionPriceId INT = (SELECT TOP 1 CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @CatalogueSolutionId AND PublishedStatusId = 3); --Emis Web GP Price
    DECLARE @AdditionalServicePriceId INT = (SELECT TOP 1 CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @AdditionalServiceId AND PublishedStatusId = 3); --Automated Arrivals Price
    DECLARE @AssociatedServicePriceId INT = (SELECT TOP 1 CataloguePriceId FROM catalogue.CataloguePrices WHERE CatalogueItemId = @AssociatedServiceId AND PublishedStatusId = 3); --Installation Price
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
        'Emis',
        'Health',
        'emisHealth@email.com',
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
    -- Completed orders: catalogue solution only, and with add on services.
    -- Both run through the same UI steps, driven by the two tables below.
    -------------------------------------------------------

    DECLARE @CompletedOrders TABLE(
        Seq INT NOT NULL,
        OrderNumber INT NOT NULL,
        Description NVARCHAR(100) NOT NULL,
        HasAddOnServices BIT NOT NULL);

    INSERT INTO @CompletedOrders (Seq, OrderNumber, Description, HasAddOnServices)
    VALUES
    (1, 5, 'catalogue solution only order', 0),
    (2, 6, 'Order with add on services', 1);

    -- Items seeded on each completed order.
    DECLARE @OrderItemsToSeed TABLE(
        Seq INT NOT NULL,
        CatalogueItemId NVARCHAR(14) NOT NULL,
        CataloguePriceId INT NULL,
        IsSolution BIT NOT NULL,
        IsAddOn BIT NOT NULL,
        EstimationPeriodId INT NOT NULL,
        QuantityB84016 INT NOT NULL,
        QuantityB84613 INT NOT NULL);

    INSERT INTO @OrderItemsToSeed (Seq, CatalogueItemId, CataloguePriceId, IsSolution, IsAddOn, EstimationPeriodId, QuantityB84016, QuantityB84613)
    VALUES
    (1, @CatalogueSolutionId, @CatalogueSolutionPriceId, 1, 0, 1, 200, 200),
    (2, @AdditionalServiceId, @AdditionalServicePriceId, 0, 1, 2, 20, 200),
    (3, @AssociatedServiceId, @AssociatedServicePriceId, 0, 1, 2, 20, 200);

    -- Scratch tables for capturing generated ids. Cleared before each use inside the loops.
    DECLARE @OrderIdTable TABLE (Id INT);
    DECLARE @OrderItemIdTable TABLE (Id INT);
    DECLARE @OrderItemPriceIdTable TABLE (Id INT);
    DECLARE @ContractIdTable TABLE (Id INT);

    DECLARE
        @OrderId INT,
        @OrderItemId INT,
        @OrderItemPriceId INT,
        @ContractId INT,
        @SolutionOrderItemId INT,
        @OrderSeq INT = 1,
        @OrderCount INT = (SELECT COUNT(*) FROM @CompletedOrders),
        @ItemSeq INT,
        @ItemCount INT = (SELECT COUNT(*) FROM @OrderItemsToSeed),
        @OrderNumber INT,
        @OrderDescription NVARCHAR(100),
        @HasAddOnServices BIT,
        @ItemCatalogueItemId NVARCHAR(14),
        @ItemCataloguePriceId INT,
        @ItemIsSolution BIT,
        @ItemIsAddOn BIT,
        @ItemEstimationPeriodId INT,
        @ItemParentId INT,
        @ItemQuantityB84016 INT,
        @ItemQuantityB84613 INT;

    WHILE @OrderSeq <= @OrderCount
    BEGIN
        SELECT
            @OrderNumber = OrderNumber,
            @OrderDescription = Description,
            @HasAddOnServices = HasAddOnServices
        FROM @CompletedOrders
        WHERE Seq = @OrderSeq;

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

        DELETE FROM @OrderIdTable;

        INSERT INTO ordering.Orders
        (OrderNumber, Revision, Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, CommencementDate,
            Created, LastUpdated, LastUpdatedBy, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly, OrderTypeId)
            OUTPUT INSERTED.Id INTO @OrderIdTable (Id)
        VALUES
        (
        @OrderNumber,
        1,
        @OrderDescription,
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

        SELECT @OrderId = Id FROM @OrderIdTable;

        -- Solutions and services: solution and delivery date on the order

        UPDATE ordering.Orders
        SET SolutionId = @CatalogueSolutionId,
            DeliveryDate = DATEADD(day, 20, SYSDATETIME())
        WHERE Id = @OrderId;

        -- Service recipients (order level, once per order)

        INSERT INTO ordering.OrderSublocations (OrderId, SublocationOdsCode, OwnerOdsCode)
        VALUES
        (@OrderId, @SublocationOdsCode, @OrderingPartyOdsCode)

        INSERT INTO ordering.OrderSublocationRecipients (OrderId, ParentSublocationOdsCode, RecipientOdsCode)
        VALUES
        (@OrderId, @SublocationOdsCode, @RecipientB84016),
        (@OrderId, @SublocationOdsCode, @RecipientB84613);

        -- Items: the catalogue solution first, then the add on services

        SET @ItemSeq = 1;
        SET @SolutionOrderItemId = NULL;

        WHILE @ItemSeq <= @ItemCount
        BEGIN
            SELECT
                @ItemCatalogueItemId = CatalogueItemId,
                @ItemCataloguePriceId = CataloguePriceId,
                @ItemIsSolution = IsSolution,
                @ItemIsAddOn = IsAddOn,
                @ItemEstimationPeriodId = EstimationPeriodId,
                @ItemQuantityB84016 = QuantityB84016,
                @ItemQuantityB84613 = QuantityB84613
            FROM @OrderItemsToSeed
            WHERE Seq = @ItemSeq;

            IF @ItemIsAddOn = 0 OR @HasAddOnServices = 1
            BEGIN
                IF @ItemIsSolution = 1
                    SET @ItemParentId = NULL;
                ELSE
                    SET @ItemParentId = @SolutionOrderItemId;

                DELETE FROM @OrderItemIdTable;

                INSERT INTO ordering.OrderItemsV2 (OrderId, CatalogueItemId, ParentId, EstimationPeriodId, Created, LastUpdated, LastUpdatedBy)
                OUTPUT INSERTED.Id INTO @OrderItemIdTable (Id)
                VALUES (@OrderId, @ItemCatalogueItemId, @ItemParentId, @ItemEstimationPeriodId, SYSDATETIME(), SYSDATETIME(), @sueId);

                SELECT @OrderItemId = Id FROM @OrderItemIdTable;

                IF @ItemIsSolution = 1
                    SET @SolutionOrderItemId = @OrderItemId;

                DELETE FROM @OrderItemPriceIdTable;

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
                WHERE CP.CataloguePriceId = @ItemCataloguePriceId

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
                WHERE CataloguePriceId = @ItemCataloguePriceId

                INSERT INTO ordering.OrderItemSublocationRecipientsV2 (OrderItemId, OrderId, ParentSublocationOdsCode, RecipientOdsCode, Quantity, DeliveryDate, LastUpdated, LastUpdatedBy)
                VALUES
                (@OrderItemId, @OrderId, @SublocationOdsCode, @RecipientB84016, @ItemQuantityB84016, DATEADD(day, 20, SYSDATETIME()), SYSDATETIME(), @sueId),
                (@OrderItemId, @OrderId, @SublocationOdsCode, @RecipientB84613, @ItemQuantityB84613, DATEADD(day, 20, SYSDATETIME()), SYSDATETIME(), @sueId);

                -- Funding type

                INSERT INTO ordering.OrderItemFundingV2 (OrderItemId, OrderItemFundingType, LastUpdated, LastUpdatedBy)
                VALUES (@OrderItemId, 2, SYSDATETIME(), @sueId);
            END

            SET @ItemSeq = @ItemSeq + 1;
        END

        -- Funding source step: framework selection on the order

        UPDATE ordering.Orders
        SET SelectedFrameworkId = @SelectedFrameworkId
        WHERE Id = @OrderId;

        -- Implementation milestone step

        IF EXISTS (SELECT 1 FROM ordering.Contracts WHERE OrderId = @OrderId)
        BEGIN
            SELECT @ContractId = Id FROM ordering.Contracts WHERE OrderId = @OrderId;
        END
        ELSE
        BEGIN
            DELETE FROM @ContractIdTable;

            INSERT INTO ordering.Contracts (OrderId)
            OUTPUT INSERTED.Id INTO @ContractIdTable (Id)
            VALUES (@OrderId);

            SELECT @ContractId = Id FROM @ContractIdTable;
        END

        INSERT INTO ordering.ImplementationPlans (ContractId, IsDefault, LastUpdated, LastUpdatedBy)
        VALUES (@ContractId, 0, SYSDATETIME(), @sueId);

        -- Associated service requirements step

        IF @HasAddOnServices = 1
        BEGIN
            INSERT INTO ordering.ContractBilling (ContractId, HasConfirmedRequirements)
            VALUES (@ContractId, 1);
        END

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

        SET @OrderSeq = @OrderSeq + 1;
    END

    UPDATE ordering.Orders SET OrderNumber = Id
END
