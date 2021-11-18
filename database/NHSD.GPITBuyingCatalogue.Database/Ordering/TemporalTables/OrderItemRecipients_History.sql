CREATE TABLE ordering.OrderItemRecipients_History
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    OdsCode nvarchar(8) NOT NULL,
    Quantity int NOT NULL,
    DeliveryDate date NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
