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
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CataloguePriceTiers_History
ON catalogue.CataloguePriceTiers_History;
GO

CREATE NONCLUSTERED INDEX IX_CataloguePriceTiers_History_Id_Period_Columns
ON catalogue.CataloguePriceTiers_History (SysEndTime, SysStartTime, Id);
GO
