CREATE TABLE catalogue.CataloguePriceTiers_History
(
    Id INT NOT NULL,
    CataloguePriceId INT NOT NULL,
    LowerRange INT NOT NULL,
    UpperRange INT NULL,
    Price DECIMAL(18, 4) NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) NOT NULL,
    SysEndTime DATETIME2(0) NOT NULL
);
