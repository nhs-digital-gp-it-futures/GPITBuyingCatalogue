CREATE TABLE catalogue.CataloguePriceTiers
(
    Id INT IDENTITY(1, 1) NOT NULL,
    CataloguePriceId INT NOT NULL,
    LowerRange INT NOT NULL CONSTRAINT CataloguePriceTiers_PositiveLowerRange CHECK (LowerRange >= 1),
    UpperRange INT NULL,
    Price DECIMAL(18, 4) NOT NULL CONSTRAINT CataloguePriceTiers_PositivePrice CHECK (Price >= 0.0000),
    LastUpdated DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CataloguePriceTiers PRIMARY KEY (Id),
    CONSTRAINT FK_CataloguePriceTiers_CataloguePriceId FOREIGN KEY (CataloguePriceId) REFERENCES catalogue.CataloguePrices(CataloguePriceId) ON DELETE CASCADE,
    CONSTRAINT FK_CataloguePriceTiers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CataloguePriceTiers_History));
