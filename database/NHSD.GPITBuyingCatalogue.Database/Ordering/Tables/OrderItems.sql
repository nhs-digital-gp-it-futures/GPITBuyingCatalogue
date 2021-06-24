CREATE TABLE ordering.OrderItems
(
    OrderId int NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    PriceId int NOT NULL,
    Price decimal(18, 4) NULL CONSTRAINT OrderItem_PositivePrice CHECK (Price >= 0.00),
    EstimationPeriodId int NULL,
    DefaultDeliveryDate date NULL,
    Created datetime2 CONSTRAINT DF_OrderItem_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated datetime2 CONSTRAINT DF_OrderItem_LastUpdated DEFAULT GETUTCDATE() NOT NULL CONSTRAINT OrderItem_LastUpdatedNotBeforeCreated CHECK (LastUpdated >= Created),
    CONSTRAINT PK_OrderItem PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItem_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES dbo.CatalogueItem (CatalogueItemId),
    CONSTRAINT FK_OrderItem_EstimationPeriod FOREIGN KEY (EstimationPeriodId) REFERENCES dbo.TimeUnit (TimeUnitId),
    CONSTRAINT FK_OrderItem_PriceId FOREIGN KEY (PriceId) REFERENCES dbo.CataloguePrice (CataloguePriceId),
);
