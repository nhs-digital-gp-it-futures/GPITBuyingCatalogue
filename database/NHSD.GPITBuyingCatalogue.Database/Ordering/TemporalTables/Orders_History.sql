﻿CREATE TABLE ordering.Orders_History
(
    Id INT NOT NULL,
    CallOffId NVARCHAR(4000) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    OrderingPartyId INT NOT NULL,
    OrderingPartyContactId INT NULL,
    SupplierId INT NULL,
    SupplierContactId INT NULL,
    CommencementDate DATE NULL,
    FundingSourceOnlyGMS BIT NULL,
    ConfirmedFundingSource BIT NULL,
    OrderTriageValueId INT NULL,
    Created DATETIME2 NOT NULL,
    LastUpdated DATETIME2 NOT NULL,
    LastUpdatedBy INT NULL,
    Completed DATETIME2 NULL ,
    OrderStatusId INT NULL,
    IsDeleted BIT NOT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
    InitialPeriod INT NULL,
    MaximumTerm INT NULL,
    AssociatedServicesOnly BIT NULL, 
    [SolutionId] NVARCHAR(14) NULL
);
