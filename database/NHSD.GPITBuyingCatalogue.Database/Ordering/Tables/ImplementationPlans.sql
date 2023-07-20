CREATE TABLE ordering.ImplementationPlans
(
    Id int IDENTITY(1, 1) NOT NULL,
    ContractId int NULL DEFAULT NULL,
    IsDefault bit NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ImplementationPlans PRIMARY KEY (Id),
    CONSTRAINT FK_ImplementationPlans_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
    CONSTRAINT FK_ImplementationPlans_Contract FOREIGN KEY (ContractId) REFERENCES ordering.Contracts(Id) ON DELETE CASCADE
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ImplementationPlans_History));
