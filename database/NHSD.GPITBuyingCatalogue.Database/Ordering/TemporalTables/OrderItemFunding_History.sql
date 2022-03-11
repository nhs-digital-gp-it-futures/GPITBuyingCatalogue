CREATE TABLE [ordering].[OrderItemFunding_History]
(
	OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    TotalPrice DECIMAL(18,4) NOT NULL,
    CentralAllocation DECIMAL(18,4) NOT NULL,
    LocalAllocation DECIMAL(18,4) NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NOT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
)
