CREATE TABLE [ordering].[OrderItemPriceTiers_History]
(
    Id INT NOT NULL,
	OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    Price DECIMAL(18,4) NOT NULL,
    LowerRange INT NOT NULL,
    UpperRange INT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL
)
