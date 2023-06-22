CREATE TABLE ordering.ContractBilling
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractId int NOT NULL, 
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ContractBilling PRIMARY KEY (Id),
    CONSTRAINT FK_ContractBilling_Contract FOREIGN KEY (ContractId) REFERENCES ordering.Contracts(Id),
    CONSTRAINT FK_ContractBilling_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ContractBilling_History));
