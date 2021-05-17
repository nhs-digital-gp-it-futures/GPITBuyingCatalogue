CREATE TABLE dbo.CataloguePrice
(
    CataloguePriceId int IDENTITY(1, 1) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    ProvisioningTypeId int NOT NULL,
    CataloguePriceTypeId int NOT NULL,
    PricingUnitId uniqueidentifier NOT NULL,
    TimeUnitId int NULL,
    CurrencyCode nvarchar(3) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    Price decimal(18, 4) NULL,
    CONSTRAINT PK_CataloguePrice PRIMARY KEY (CataloguePriceId),
    CONSTRAINT FK_CataloguePrice_CatalogueItem_CatalogueItemId FOREIGN KEY (CatalogueItemId) REFERENCES dbo.CatalogueItem(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_CataloguePrice_ProvisioningType_ProvisioningTypeId FOREIGN KEY (ProvisioningTypeId) REFERENCES dbo.ProvisioningType(ProvisioningTypeId),
    CONSTRAINT FK_CataloguePrice_CataloguePriceType_CataloguePriceTypeId FOREIGN KEY (CataloguePriceTypeId) REFERENCES dbo.CataloguePriceType(CataloguePriceTypeId),
    CONSTRAINT FK_CataloguePrice_PricingUnit_PricingUnitId FOREIGN KEY (PricingUnitId) REFERENCES dbo.PricingUnit(PricingUnitId),
    CONSTRAINT FK_CataloguePrice_TimeUnit_TimeUnitId FOREIGN KEY (TimeUnitId) REFERENCES dbo.TimeUnit(TimeUnitId),
);
