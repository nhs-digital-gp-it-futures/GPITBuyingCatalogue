CREATE TABLE catalogue.CataloguePriceTiers
(
    Id int IDENTITY(1,1) NOT NULL,
    CataloguePriceId int NOT NULL,
    BandStart int NOT NULL,
    BandEnd int NULL,
    Price decimal(18,3) NOT NULL,
    CONSTRAINT PK_CataloguePriceTier PRIMARY KEY (Id),
    CONSTRAINT FK_CataloguePriceTier_CataloguePrice_CataloguePriceId FOREIGN KEY (CataloguePriceId) REFERENCES catalogue.CataloguePrices(CataloguePriceId) ON DELETE CASCADE
);
