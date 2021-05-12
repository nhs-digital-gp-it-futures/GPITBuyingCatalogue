CREATE TABLE dbo.CataloguePriceTier
(
    CataloguePriceTierId int IDENTITY(1,1) NOT NULL,
    CataloguePriceId int NOT NULL,
    BandStart int NOT NULL,
    BandEnd int NULL,
    Price decimal(18,3) NOT NULL,
    CONSTRAINT PK_CataloguePriceTier PRIMARY KEY (CataloguePriceTierId),
    CONSTRAINT FK_CataloguePriceTier_CataloguePrice_CataloguePriceId FOREIGN KEY (CataloguePriceId) REFERENCES dbo.CataloguePrice(CataloguePriceId) ON DELETE CASCADE
);
