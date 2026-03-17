CREATE TABLE ordering.OrderItemsV2_History
(
    Id INT NOT NULL,
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    PriceId int NULL,
    Price decimal(18, 4) NULL,
    ParentId INT NULL,
    EstimationPeriodId int NULL,
    DefaultDeliveryDate date NULL,
    Created datetime2 NOT NULL,
    LastUpdated datetime2 NOT NULL ,
    LastUpdatedBy INT NULL, 
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL, 
    [Quantity] INT NULL,   
);
