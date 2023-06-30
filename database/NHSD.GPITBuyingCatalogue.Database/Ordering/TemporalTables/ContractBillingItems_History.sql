CREATE TABLE ordering.ContractBillingItems_History
(
    Id int NOT NULL,
    ContractBillingId int NOT NULL, 
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    Quantity int NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
