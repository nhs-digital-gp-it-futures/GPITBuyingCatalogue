CREATE TABLE [ordering].[OrderItemSublocationRecipientsV2]
(
    [Id] INT IDENTITY(1, 1) NOT NULL,
    [OrderItemId] INT NOT NULL,
    [OrderSublocationRecipientId] INT NOT NULL,
    [Quantity] int NULL CONSTRAINT PositiveQuantityV2_OrderItemSublocationRecipients_Quantity CHECK (Quantity >= 0),
    [DeliveryDate] date NULL,
    [LastUpdated] datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    [LastUpdatedBy] int NULL,
    [SysStartTime] datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemSublocationRecipientsV2 PRIMARY KEY ([Id]),
    CONSTRAINT UQ_OrderItemSublocationRecipientsV2 UNIQUE ([OrderItemId], [OrderSublocationRecipientId]),
    CONSTRAINT FK_OrderItemSublocationRecipientsV2_OrderItem FOREIGN KEY ([OrderItemId]) REFERENCES [ordering].[OrderItemsV2] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemSublocationRecipientsV2_SublocationRecipient FOREIGN KEY ([OrderSublocationRecipientId]) REFERENCES [ordering].[OrderSublocationRecipientsV2] ([Id]) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItemSublocationRecipientsV2_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES [users].[AspNetUsers]([Id]),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [ordering].[OrderItemSublocationRecipientsV2_History]))
