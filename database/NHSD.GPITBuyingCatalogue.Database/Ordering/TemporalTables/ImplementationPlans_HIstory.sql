CREATE TABLE ordering.ImplementationPlans_History
(
    Id int NOT NULL,
    ContractId int NULL,
    IsDefault bit NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
