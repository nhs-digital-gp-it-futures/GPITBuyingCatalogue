CREATE TABLE ordering.OrderTermination_History
(
    Id int NOT NULL,
    OrderId INT NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    DateOfTermination DATETIME2(0) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);

