CREATE TABLE ordering.Contracts_History
(
    Id int NOT NULL,
    OrderId int NOT NULL,
    ImplementationPlanId int NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL, 
);
