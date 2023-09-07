CREATE TABLE ordering.OrderTermination
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId INT NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    DateOfTermination DATETIME2(0) NOT NULL,
    CONSTRAINT PK_OrderTermination PRIMARY KEY (Id),
    CONSTRAINT FK_OrderTermination_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id) ON DELETE CASCADE
);

