CREATE TABLE [ordering].[OrderItemFunding]
(
    OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    OrderItemFundingType INT NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemFunding PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_OrderItemFunding_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id),
    CONSTRAINT FK_OrderItemFunding_OrderItem FOREIGN KEY(OrderId, CatalogueItemId) REFERENCES ordering.OrderItems(OrderId, CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemFunding_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems (Id),
    CONSTRAINT FK_OrderItemFunding_OrderItemFundingType FOREIGN KEY (OrderItemFundingType) REFERENCES ordering.OrderItemFundingTypes(Id),
    CONSTRAINT FK_OrderItemFunding_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.OrderItemFunding_History));
