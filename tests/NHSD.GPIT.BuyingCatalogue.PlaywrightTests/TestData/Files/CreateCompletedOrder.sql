-- Creates a fresh, fully completed order for Amend Journey tests.
-- @OrderDescription and @HasAddOnServices are supplied by the test caller.

DECLARE @sueId INT, @OrderingParty INT;

SELECT
    @sueId = Id,
    @OrderingParty = PrimaryOrganisationId
FROM users.AspNetUsers
WHERE UserName = 'SueSmith@email.com';


DECLARE @OrderingPartyOdsCode NVARCHAR(10);

SELECT @OrderingPartyOdsCode = ExternalIdentifier
FROM organisations.Organisations
WHERE Id = @OrderingParty;


DECLARE
    @SupplierId INT = 10000,
    @CatalogueSolutionId NVARCHAR(14) = '10000-001',
    @AdditionalServiceId NVARCHAR(14) = '10000-001A003',
    @AssociatedServiceId NVARCHAR(14) = '10000-S-002',
    @LastBuyerContactId INT,
    @LastSupplierContactId INT;


DECLARE
    @SublocationOdsCode NVARCHAR(10) = '02T',
    @RecipientB84016 NVARCHAR(10) = 'B84016',
    @RecipientB84613 NVARCHAR(10) = 'B84613';


DECLARE @CatalogueSolutionPriceId INT =
(
    SELECT TOP 1 CataloguePriceId
    FROM catalogue.CataloguePrices
    WHERE CatalogueItemId = @CatalogueSolutionId
      AND PublishedStatusId = 3
);

DECLARE @AdditionalServicePriceId INT =
(
    SELECT TOP 1 CataloguePriceId
    FROM catalogue.CataloguePrices
    WHERE CatalogueItemId = @AdditionalServiceId
      AND PublishedStatusId = 3
);

DECLARE @AssociatedServicePriceId INT =
(
    SELECT TOP 1 CataloguePriceId
    FROM catalogue.CataloguePrices
    WHERE CatalogueItemId = @AssociatedServiceId
      AND PublishedStatusId = 3
);

DECLARE @SelectedFrameworkId NVARCHAR(10) =
(
    SELECT Id
    FROM catalogue.Frameworks
    WHERE Id = 'TIF001'
);


IF @CatalogueSolutionPriceId IS NULL
    THROW 50001,
    'CreateCompletedOrder.sql: no published price found for the Emis Web GP solution.',
    1;


-- Create fresh buyer and supplier contacts for this order.

INSERT INTO ordering.Contacts
(
    FirstName,
    LastName,
    Email,
    Phone,
    LastUpdated,
    LastUpdatedBy
)
VALUES
(
    'Sue',
    'Smith',
    'SueSmith@email.com',
    '1234567',
    SYSDATETIME(),
    @sueId
);

SET @LastBuyerContactId = SCOPE_IDENTITY();


INSERT INTO ordering.Contacts
(
    FirstName,
    LastName,
    Email,
    Phone,
    LastUpdated,
    LastUpdatedBy
)
VALUES
(
    'Emis',
    'Health',
    'emisHealth@email.com',
    '1234567',
    SYSDATETIME(),
    @sueId
);

SET @LastSupplierContactId = SCOPE_IDENTITY();


DECLARE @OrderIdTable TABLE (Id INT);
DECLARE @OrderId INT;


INSERT INTO ordering.Orders
(
    OrderNumber,
    Revision,
    Description,
    OrderingPartyId,
    OrderingPartyContactId,
    SupplierId,
    SupplierContactId,
    CommencementDate,
    Created,
    LastUpdated,
    LastUpdatedBy,
    IsDeleted,
    InitialPeriod,
    MaximumTerm,
    AssociatedServicesOnly,
    OrderTypeId
)
OUTPUT INSERTED.Id INTO @OrderIdTable (Id)
VALUES
(
    1,
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
    0,
    1
);


SELECT @OrderId = Id
FROM @OrderIdTable;


-- Use the generated order Id as the OrderNumber and only update this order.

UPDATE ordering.Orders
SET
    OrderNumber = @OrderId,
    SolutionId = @CatalogueSolutionId,
    DeliveryDate = DATEADD(day, 20, SYSDATETIME())
WHERE Id = @OrderId;


INSERT INTO ordering.OrderSublocations
(
    OrderId,
    SublocationOdsCode,
    OwnerOdsCode
)
VALUES
(
    @OrderId,
    @SublocationOdsCode,
    @OrderingPartyOdsCode
);


INSERT INTO ordering.OrderSublocationRecipients
(
    OrderId,
    ParentSublocationOdsCode,
    RecipientOdsCode
)
VALUES
    (@OrderId, @SublocationOdsCode, @RecipientB84016),
    (@OrderId, @SublocationOdsCode, @RecipientB84613);

-- Add-on services are only added when @HasAddOnServices = 1.

DECLARE @OrderItemsToSeed TABLE
(
    Seq INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    CataloguePriceId INT NULL,
    IsSolution BIT NOT NULL,
    IsAddOn BIT NOT NULL,
    EstimationPeriodId INT NOT NULL,
    QuantityB84016 INT NOT NULL,
    QuantityB84613 INT NOT NULL
);


INSERT INTO @OrderItemsToSeed
(
    Seq,
    CatalogueItemId,
    CataloguePriceId,
    IsSolution,
    IsAddOn,
    EstimationPeriodId,
    QuantityB84016,
    QuantityB84613
)
VALUES
    (1, @CatalogueSolutionId, @CatalogueSolutionPriceId, 1, 0, 1, 200, 200),
    (2, @AdditionalServiceId, @AdditionalServicePriceId, 0, 1, 2, 20, 200),
    (3, @AssociatedServiceId, @AssociatedServicePriceId, 0, 1, 2, 20, 200);


DECLARE @OrderItemIdTable TABLE (Id INT);
DECLARE @OrderItemPriceIdTable TABLE (Id INT);
DECLARE @ContractIdTable TABLE (Id INT);


DECLARE
    @OrderItemId INT,
    @OrderItemPriceId INT,
    @ContractId INT,
    @SolutionOrderItemId INT = NULL,
    @ItemSeq INT = 1,
    @ItemCount INT = (SELECT COUNT(*) FROM @OrderItemsToSeed),
    @ItemCatalogueItemId NVARCHAR(14),
    @ItemCataloguePriceId INT,
    @ItemIsSolution BIT,
    @ItemIsAddOn BIT,
    @ItemEstimationPeriodId INT,
    @ItemParentId INT,
    @ItemQuantityB84016 INT,
    @ItemQuantityB84613 INT;


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

-- Add-on services are children of the catalogue solution.

        IF @ItemIsSolution = 1
            SET @ItemParentId = NULL;
        ELSE
            SET @ItemParentId = @SolutionOrderItemId;


        DELETE FROM @OrderItemIdTable;


        INSERT INTO ordering.OrderItemsV2
        (
            OrderId,
            CatalogueItemId,
            ParentId,
            EstimationPeriodId,
            Created,
            LastUpdated,
            LastUpdatedBy
        )
        OUTPUT INSERTED.Id INTO @OrderItemIdTable (Id)
        VALUES
        (
            @OrderId,
            @ItemCatalogueItemId,
            @ItemParentId,
            @ItemEstimationPeriodId,
            SYSDATETIME(),
            SYSDATETIME(),
            @sueId
        );


        SELECT @OrderItemId = Id
        FROM @OrderItemIdTable;


        IF @ItemIsSolution = 1
            SET @SolutionOrderItemId = @OrderItemId;


        DELETE FROM @OrderItemPriceIdTable;


        INSERT INTO ordering.OrderItemPricesV2
        (
            OrderItemId,
            CataloguePriceId,
            BillingPeriodId,
            ProvisioningTypeId,
            CataloguePriceTypeId,
            CataloguePriceCalculationTypeId,
            CataloguePriceQuantityCalculationTypeId,
            CurrencyCode,
            Description,
            RangeDescription,
            LastUpdated,
            LastUpdatedBy
        )
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
        WHERE CP.CataloguePriceId = @ItemCataloguePriceId;


        SELECT @OrderItemPriceId = Id
        FROM @OrderItemPriceIdTable;


        INSERT INTO ordering.OrderItemPriceTiers
        (
            OrderItemPriceId,
            Price,
            ListPrice,
            LowerRange,
            UpperRange,
            LastUpdated,
            LastUpdatedBy
        )
        SELECT
            @OrderItemPriceId,
            Price,
            Price,
            LowerRange,
            UpperRange,
            SYSDATETIME(),
            @sueId
        FROM catalogue.CataloguePriceTiers
        WHERE CataloguePriceId = @ItemCataloguePriceId;


        INSERT INTO ordering.OrderItemSublocationRecipientsV2
        (
            OrderItemId,
            OrderId,
            ParentSublocationOdsCode,
            RecipientOdsCode,
            Quantity,
            DeliveryDate,
            LastUpdated,
            LastUpdatedBy
        )
        VALUES
            (
                @OrderItemId,
                @OrderId,
                @SublocationOdsCode,
                @RecipientB84016,
                @ItemQuantityB84016,
                DATEADD(day, 20, SYSDATETIME()),
                SYSDATETIME(),
                @sueId
            ),
            (
                @OrderItemId,
                @OrderId,
                @SublocationOdsCode,
                @RecipientB84613,
                @ItemQuantityB84613,
                DATEADD(day, 20, SYSDATETIME()),
                SYSDATETIME(),
                @sueId
            );


        INSERT INTO ordering.OrderItemFundingV2
        (
            OrderItemId,
            OrderItemFundingType,
            LastUpdated,
            LastUpdatedBy
        )
        VALUES
        (
            @OrderItemId,
            2,
            SYSDATETIME(),
            @sueId
        );

    END


    SET @ItemSeq = @ItemSeq + 1;

END


UPDATE ordering.Orders
SET SelectedFrameworkId = @SelectedFrameworkId
WHERE Id = @OrderId;


IF EXISTS
(
    SELECT 1
    FROM ordering.Contracts
    WHERE OrderId = @OrderId
)
BEGIN

    SELECT @ContractId = Id
    FROM ordering.Contracts
    WHERE OrderId = @OrderId;

END
ELSE
BEGIN

    INSERT INTO ordering.Contracts (OrderId)
    OUTPUT INSERTED.Id INTO @ContractIdTable (Id)
    VALUES (@OrderId);


    SELECT @ContractId = Id
    FROM @ContractIdTable;

END


INSERT INTO ordering.ImplementationPlans
(
    ContractId,
    IsDefault,
    LastUpdated,
    LastUpdatedBy
)
VALUES
(
    @ContractId,
    0,
    SYSDATETIME(),
    @sueId
);


IF @HasAddOnServices = 1
BEGIN

    INSERT INTO ordering.ContractBilling
    (
        ContractId,
        HasConfirmedRequirements
    )
    VALUES
    (
        @ContractId,
        1
    );

END


IF NOT EXISTS
(
    SELECT 1
    FROM ordering.ContractFlags
    WHERE OrderId = @OrderId
)
BEGIN

    INSERT INTO ordering.ContractFlags
    (
        OrderId,
        UseDefaultDataProcessing,
        LastUpdated,
        LastUpdatedBy
    )
    VALUES
    (
        @OrderId,
        1,
        SYSDATETIME(),
        @sueId
    );

END


UPDATE ordering.Orders
SET AcceptedTermsAndConditions = 1
WHERE Id = @OrderId;


UPDATE ordering.Orders
SET Completed = SYSDATETIME()
WHERE Id = @OrderId;


-- Return the newly created order to the Playwright test.

SELECT
    @OrderId AS OrderId,
    @OrderId AS OrderNumber;
