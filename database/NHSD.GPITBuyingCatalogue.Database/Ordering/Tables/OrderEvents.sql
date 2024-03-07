CREATE TABLE ordering.OrderEvents
(
    Id INT IDENTITY(1, 1) NOT NULL,
    OrderId INT NOT NULL,
    EventId INT NOT NULL,
    CONSTRAINT PK_OrderEvents PRIMARY KEY (Id),
    CONSTRAINT FK_OrderEvents_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders (Id) ON DELETE CASCADE,
    CONSTRAINT FK_OrderEvents_Event FOREIGN KEY (EventId) REFERENCES notifications.Events (Id),
);

