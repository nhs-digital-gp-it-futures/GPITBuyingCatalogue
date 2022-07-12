CREATE TABLE catalogue.CataloguePrices_History
(
    CataloguePriceId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    ProvisioningTypeId INT NOT NULL,
    CataloguePriceTypeId INT NOT NULL,
    PricingUnitId SMALLINT NOT NULL,
    TimeUnitId INT NULL,
    CurrencyCode NVARCHAR(3) NOT NULL,
    PublishedStatusId INT CONSTRAINT DF_CataloguePrice_History_PublishedStatus DEFAULT 1 NOT NULL,
    IsLocked BIT CONSTRAINT DF_CataloguePrices_History_IsLocked DEFAULT 0 NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    Price DECIMAL(18, 4) NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL,
    CataloguePriceCalculationTypeId INT NULL,
    CataloguePriceQuantityCalculationTypeId INT NULL,
);
