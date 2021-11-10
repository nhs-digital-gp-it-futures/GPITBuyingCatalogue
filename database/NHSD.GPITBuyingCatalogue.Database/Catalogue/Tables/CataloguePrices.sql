﻿CREATE TABLE catalogue.CataloguePrices
(
    CataloguePriceId int IDENTITY(1, 1) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    ProvisioningTypeId int NOT NULL,
    CataloguePriceTypeId int NOT NULL,
    PricingUnitId smallint NOT NULL,
    TimeUnitId int NULL,
    CurrencyCode nvarchar(3) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    Price decimal(18, 4) NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CataloguePrices PRIMARY KEY (CataloguePriceId),
    CONSTRAINT FK_CataloguePrices_CatalogueItem_CatalogueItemId FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CataloguePrices_ProvisioningType_ProvisioningTypeId FOREIGN KEY (ProvisioningTypeId) REFERENCES catalogue.ProvisioningTypes(Id),
    CONSTRAINT FK_CataloguePrices_CataloguePriceType_CataloguePriceTypeId FOREIGN KEY (CataloguePriceTypeId) REFERENCES catalogue.CataloguePriceTypes(Id),
    CONSTRAINT FK_CataloguePrices_PricingUnit_PricingUnitId FOREIGN KEY (PricingUnitId) REFERENCES catalogue.PricingUnits(Id),
    CONSTRAINT FK_CataloguePrices_TimeUnit_TimeUnitId FOREIGN KEY (TimeUnitId) REFERENCES catalogue.TimeUnits(Id),
    CONSTRAINT FK_CataloguePrices_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
