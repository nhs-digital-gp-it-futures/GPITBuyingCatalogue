CREATE TABLE ordering.DefaultDeliveryDates_History
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    DeliveryDate date NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_DefaultDeliveryDates_History
ON ordering.DefaultDeliveryDates_History;
GO

CREATE NONCLUSTERED INDEX IX_DefaultDeliveryDates_History_OrderId_CatalogueItemId_Period_Columns
ON ordering.DefaultDeliveryDates_History (SysEndTime, SysStartTime, CatalogueItemId, OrderId);
GO
