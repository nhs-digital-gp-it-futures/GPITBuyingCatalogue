CREATE TABLE [dbo].[OrderItemPrices_History]
(
    OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    [CataloguePriceId] INT NOT NULL,
    BillingPeriodId INT NULL,
    ProvisioningTypeId INT NOT NULL,
    CataloguePriceTypeId INT NOT NULL,
    CataloguePriceCalculationTypeId INT NOT NULL,
    CataloguePriceQuantityCalculationTypeId INT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    [RangeDescription] NVARCHAR(100) NULL,
    LastUpdated DATETIME2(0) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
)
