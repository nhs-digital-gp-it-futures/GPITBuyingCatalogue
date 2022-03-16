CREATE TABLE [dbo].[OrderItemPrices_History]
(
	OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    EstimationPeriodId INT NOT NULL,
    ProvisioningTypeId INT NOT NULL,
    CataloguePriceTypeId INT NOT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    LastUpdated DATETIME2(0) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
)
