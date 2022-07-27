CREATE TABLE ordering.ContractFlags
(
    Id int IDENTITY(1, 1) NOT NULL,
    OrderId int NOT NULL, 
    HasSpecificRequirements bit NULL,
    UseDefaultBilling bit NULL,
    UseDefaultDataProcessing bit NULL,
    UseDefaultImplementationPlan bit NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ContractFlags PRIMARY KEY (Id),
    CONSTRAINT FK_ContractFlags_Order FOREIGN KEY (OrderId) REFERENCES ordering.Orders(Id),
    CONSTRAINT FK_ContractFlags_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ContractFlags_History));
