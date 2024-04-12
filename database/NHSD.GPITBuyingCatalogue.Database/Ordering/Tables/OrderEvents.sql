CREATE TABLE ordering.OrderEvents
(
    Id INT IDENTITY(1, 1) NOT NULL,
    OrderNumber INT NULL,
    OrderId INT NULL,
    EventTypeId INT NOT NULL,
    [Created] DATETIME2 CONSTRAINT DF_OrderEvents_Created DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_OrderEvents PRIMARY KEY (Id),
    CONSTRAINT FK_OrderEvents_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderEvents_EventType FOREIGN KEY (EventTypeId) REFERENCES notifications.EventTypes (Id),
);
