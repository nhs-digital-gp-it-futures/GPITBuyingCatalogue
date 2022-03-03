CREATE TABLE ordering.Orders_History
(
    Id int NOT NULL,
    CallOffId NVARCHAR(4000) NOT NULL,
    [Description] nvarchar(100) NOT NULL,
    OrderingPartyId int NOT NULL,
    OrderingPartyContactId int NULL,
    SupplierId int NULL,
    SupplierContactId int NULL,
    CommencementDate date NULL,
    FundingSourceOnlyGMS bit NULL,
    ConfirmedFundingSource bit NULL,
    Created datetime2 NOT NULL,
    LastUpdated datetime2 NOT NULL,
    LastUpdatedBy int NULL,
    Completed datetime2 NULL ,
    OrderStatusId int NOT NULL,
    IsDeleted bit NOT NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL, 
    [InitialPeriod] INT NULL, 
    [MaximumTerm] INT NULL  
);
