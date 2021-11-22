CREATE TABLE catalogue.CataloguePriceTiers_History
(
    Id int NOT NULL,
    CataloguePriceId int NOT NULL,
    BandStart int NOT NULL,
    BandEnd int NULL,
    Price decimal(18, 3) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
