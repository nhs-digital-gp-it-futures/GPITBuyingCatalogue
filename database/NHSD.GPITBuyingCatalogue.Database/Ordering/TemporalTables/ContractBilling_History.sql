CREATE TABLE ordering.ContractBilling_History
(
    Id int NOT NULL,
    ContractId int NOT NULL, 
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
