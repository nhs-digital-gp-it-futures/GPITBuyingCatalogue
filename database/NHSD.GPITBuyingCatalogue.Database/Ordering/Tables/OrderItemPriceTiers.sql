CREATE TABLE [ordering].[OrderItemPriceTiers]
(
    Id INT IDENTITY(1, 1) NOT NULL,
    OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    Price DECIMAL(18,4) NOT NULL CONSTRAINT OrderItemPriceTiers_PositivePrice CHECK (Price >= 0.00),
    LowerRange INT NOT NULL CONSTRAINT OrderItemPriceTiers_PositiveLowerRange CHECK (LowerRange >= 1),
    UpperRange INT NULL,
    LastUpdated DATETIME2(7) NOT NULL CONSTRAINT DF_OrderItemPriceTiers_Default DEFAULT GETUTCDATE(),
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemPriceTiers PRIMARY KEY (Id),
    CONSTRAINT FK_OrderItemPriceTiers_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id),
    CONSTRAINT FK_OrderItemPriceTiers_OrderItems FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId),
    CONSTRAINT FK_OrderItemPriceTiers_OrderItemPrices FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItemPrices(OrderId, CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemPriceTiers_CatalogueItemId FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems (Id),
    CONSTRAINT FK_OrderItemPriceTiers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.OrderItemPriceTiers_History));
