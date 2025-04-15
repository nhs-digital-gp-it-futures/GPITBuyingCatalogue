CREATE TABLE [ordering].[OrderItemSublocationRecipients]
(
    [OrderId] INT NOT NULL,
    [CatalogueItemId] nvarchar(14) NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] int NULL CONSTRAINT PositiveQuantity_OrderItemSublocationRecipients_Quantity CHECK (Quantity > 0),
    [DeliveryDate] date NULL,
    [LastUpdated] datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    [LastUpdatedBy] int NULL,
    [SysStartTime] datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemSublocationRecipients PRIMARY KEY ([OrderId], [CatalogueItemId] , [RecipientOdsCode]),
    CONSTRAINT FK_OrderItemSublocationRecipients_OrderItem FOREIGN KEY ([OrderId], [CatalogueItemId]) REFERENCES [ordering].[OrderItems] ([OrderId], [CatalogueItemId]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemSublocationRecipients_SublocationRecipient FOREIGN KEY ([OrderId], [RecipientOdsCode]) REFERENCES [ordering].[OrderSublocationRecipients] ([OrderId], [RecipientOdsCode]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemSublocationRecipients_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES [users].[AspNetUsers]([Id]),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [ordering].[OrderItemSublocationRecipients_History]))
