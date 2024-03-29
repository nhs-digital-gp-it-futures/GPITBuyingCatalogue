﻿CREATE TABLE ordering.OrderItems
(
    OrderId INT NOT NULL,
    CatalogueItemId NVARCHAR(14) NOT NULL,
    PriceId INT NULL,
    Price DECIMAL(18, 4) NULL CONSTRAINT OrderItem_PositivePrice CHECK (Price >= 0.00),
    EstimationPeriodId INT NULL,
    DefaultDeliveryDate DATE NULL,
    Created DATETIME2 CONSTRAINT DF_OrderItem_Created DEFAULT GETUTCDATE() NOT NULL,
    LastUpdated DATETIME2 CONSTRAINT DF_OrderItem_LastUpdated DEFAULT GETUTCDATE() NOT NULL CONSTRAINT OrderItem_LastUpdatedNotBeforeCreated CHECK (LastUpdated >= Created),
    LastUpdatedBy INT NULL, 
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    [Quantity] INT NULL CONSTRAINT OrderItem_PositiveQuantity CHECK (Quantity > 0),
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, CatalogueItemId),
    CONSTRAINT FK_OrderItems_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderItems_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems (Id),
    CONSTRAINT FK_OrderItems_EstimationPeriod FOREIGN KEY (EstimationPeriodId) REFERENCES catalogue.TimeUnits (Id),
    CONSTRAINT FK_OrderItems_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers (Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.OrderItems_History));
