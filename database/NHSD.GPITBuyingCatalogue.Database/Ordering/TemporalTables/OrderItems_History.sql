CREATE TABLE ordering.OrderItems_History
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    PriceId int NOT NULL,
    Price decimal(18, 4) NULL,
    EstimationPeriodId int NULL,
    DefaultDeliveryDate date NULL,
    Created datetime2 NOT NULL,
    LastUpdated datetime2 NOT NULL ,
    LastUpdatedBy INT NULL, 
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
    ProvisioningTypeId INT NULL,
    CataloguePriceTypeId INT NULL,
    CurrencyCode NVARCHAR(3) NULL,    
);
