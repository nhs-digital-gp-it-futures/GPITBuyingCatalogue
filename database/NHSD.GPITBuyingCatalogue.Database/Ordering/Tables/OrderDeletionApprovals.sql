CREATE TABLE ordering.OrderDeletionApprovals
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId INT NOT NULL,
    NameOfRequester NVARCHAR(100) NOT NULL,
    NameOfApprover NVARCHAR(100) NOT NULL,
    DateOfApproval DATETIME2(0) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_OrderDeletionApprovals PRIMARY KEY (Id),
    CONSTRAINT FK_OrderDeletionApprovals_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id),
    CONSTRAINT FK_OrderDeletionApprovals_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
);
