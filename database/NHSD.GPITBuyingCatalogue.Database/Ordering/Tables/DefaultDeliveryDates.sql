CREATE TABLE ordering.DefaultDeliveryDates
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    DeliveryDate date NOT NULL,
    CONSTRAINT PK_DefaultDeliveryDates PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_DefaultDeliveryDates_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
);
