CREATE TABLE [ordering].[OrderItemFundingV2]
(
    Id INT IDENTITY(1, 1) NOT NULL,
    OrderItemId INT NOT NULL,
    OrderItemFundingType INT NOT NULL,
    LastUpdated DATETIME2(7) NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderItemFundingV2 PRIMARY KEY (Id),
    CONSTRAINT FK_OrderItemFunding_OrderItemV2 FOREIGN KEY (OrderItemId) REFERENCES ordering.OrderItemsV2 (Id) ON DELETE CASCADE,
    CONSTRAINT UQ_OrderItemFunding UNIQUE (OrderItemId),
    CONSTRAINT FK_OrderItemFunding_OrderItemFundingTypeV2 FOREIGN KEY (OrderItemFundingType) REFERENCES ordering.OrderItemFundingTypes(Id),
    CONSTRAINT FK_OrderItemFunding_LastUpdatedByV2 FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers (Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.OrderItemFundingV2_History));
