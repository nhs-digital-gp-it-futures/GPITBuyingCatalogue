CREATE TABLE ordering.ContractFlags_History
(
    Id int NOT NULL,
    OrderId int NOT NULL, 
    HasSpecificRequirements bit NULL,
    UseDefaultBilling bit NULL,
    UseDefaultDataProcessing bit NULL,
    UseDefaultImplementationPlan bit NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
