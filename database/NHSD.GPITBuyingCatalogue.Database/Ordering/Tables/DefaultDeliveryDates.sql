CREATE TABLE ordering.DefaultDeliveryDates
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    DeliveryDate date NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_DefaultDeliveryDates PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_DefaultDeliveryDates_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_DefaultDeliveryDates_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.DefaultDeliveryDates_History));
