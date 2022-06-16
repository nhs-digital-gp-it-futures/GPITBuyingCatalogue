CREATE TABLE [ordering].[OrderItemFunding_History]
(
    OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    OrderItemFundingType INT NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
)
