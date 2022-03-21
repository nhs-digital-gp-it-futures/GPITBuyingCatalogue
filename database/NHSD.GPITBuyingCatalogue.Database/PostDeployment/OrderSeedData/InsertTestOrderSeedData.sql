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
        @FundingSource BIT = 1,
        @ConfirmedFundingSource BIT = 0,
        @OrderStatus INT = 2,
        @SupplierId INT = 99999, --notEmis Health
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

    --Order with description

    INSERT INTO ordering.Orders 
    (Description, OrderingPartyId, FundingSourceOnlyGMS, ConfirmedFundingSource, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
    VALUES
    (
    'Order with description',
    @OrderingParty,
    @FundingSource,
    @ConfirmedFundingSource,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

    --Order with description and ordering party contact

    INSERT INTO ordering.Contacts (FirstName, LastName, Email, Phone, LastUpdated, LastUpdatedBy)
    SELECT
    FirstName, LastName, Email, Phone, SYSDATETIME(), @sueId
    FROM @TestOrdersContacts
    WHERE Id = 1 -- buyer contact id

    SELECT @LastBuyerContactId = IDENT_CURRENT('ordering.Contacts');

    INSERT INTO ordering.Orders 
    (Description, OrderingPartyId, OrderingPartyContactId, FundingSourceOnlyGMS, ConfirmedFundingSource, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
    VALUES
    (
    'Order with description and ordering party contact',
    @OrderingParty,
    @LastBuyerContactId,
    @FundingSource,
    @ConfirmedFundingSource,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

    --Order with description, ordering party contact and supplier with it's contact

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
    (Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, FundingSourceOnlyGMS,
     ConfirmedFundingSource, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, AssociatedServicesOnly)
    VALUES
    (
    'Order with description, ordering party contact and supplier with contact',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    @FundingSource,
    @ConfirmedFundingSource,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    @AssociatedServicesOnly);

    --Order with description, ordering party contact, supplier with it's contact and timescales

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
    (Description, OrderingPartyId, OrderingPartyContactId, SupplierId, SupplierContactId, CommencementDate, FundingSourceOnlyGMS,
     ConfirmedFundingSource, Created, LastUpdated, LastUpdatedBy, OrderStatusId, IsDeleted, InitialPeriod, MaximumTerm, AssociatedServicesOnly)
    VALUES
    (
    'Order with description, ordering party contact and supplier with contact and timescales',
    @OrderingParty,
    @LastBuyerContactId,
    @SupplierId,
    @LastSupplierContactId,
    DATEADD(day, 1, SYSDATETIME()),
    @FundingSource,
    @ConfirmedFundingSource,
    SYSDATETIME(),
    SYSDATETIME(),
    @sueId,
    @OrderStatus,
    0,
    6,
    36,
    @AssociatedServicesOnly);    
END
