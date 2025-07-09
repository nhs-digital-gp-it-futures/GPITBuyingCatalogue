CREATE TABLE [ordering].[OrderItemSublocationRecipients_History]
(
    [OrderId] INT NOT NULL,
    [CatalogueItemId] nvarchar(14) NOT NULL,
    [ParentSublocationOdsCode] NVARCHAR(10) NOT NULL,
    [RecipientOdsCode] NVARCHAR(10) NOT NULL,
    [Quantity] int,
    [DeliveryDate] date NULL,
    [LastUpdated] datetime2(7) NOT NULL,
    [LastUpdatedBy] int NULL,
    [SysStartTime] datetime2(0) NOT NULL,
    [SysEndTime] datetime2(0) NOT NULL,
)