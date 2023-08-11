CREATE TABLE ordering.OrderTermination
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId INT NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    DateOfTermination DATETIME2(0) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderTermination PRIMARY KEY (Id),
    CONSTRAINT FK_OrderTermination_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id),
    CONSTRAINT FK_OrderTermination_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.OrderTermination_History));

