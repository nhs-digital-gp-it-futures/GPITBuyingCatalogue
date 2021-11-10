CREATE TABLE catalogue.CataloguePriceTiers
(
    Id int IDENTITY(1, 1) NOT NULL,
    CataloguePriceId int NOT NULL,
    BandStart int NOT NULL,
    BandEnd int NULL,
    Price decimal(18, 3) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CataloguePriceTiers PRIMARY KEY (Id),
    CONSTRAINT FK_CataloguePriceTiers_CataloguePriceId FOREIGN KEY (CataloguePriceId) REFERENCES catalogue.CataloguePrices(CataloguePriceId) ON DELETE CASCADE,
    CONSTRAINT FK_CataloguePriceTiers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
