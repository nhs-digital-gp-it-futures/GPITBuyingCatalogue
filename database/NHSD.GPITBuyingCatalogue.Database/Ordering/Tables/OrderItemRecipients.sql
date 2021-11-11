CREATE TABLE ordering.OrderItemRecipients
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    OdsCode nvarchar(8) NOT NULL,
    Quantity int NOT NULL CONSTRAINT ServiceRecipient_PositiveQuantity CHECK (Quantity > 0),
    DeliveryDate date NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemRecipients PRIMARY KEY (OrderId, CatalogueItemId, OdsCode),
    CONSTRAINT FK_OrderItemRecipients_OrderItem FOREIGN KEY (OrderId, CatalogueItemId) REFERENCES ordering.OrderItems (OrderId, CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemRecipients_OdsCode FOREIGN KEY (OdsCode) REFERENCES ordering.ServiceRecipients (OdsCode),
    CONSTRAINT FK_OrderItemRecipients_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
