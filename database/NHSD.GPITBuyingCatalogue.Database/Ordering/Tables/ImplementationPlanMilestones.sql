CREATE TABLE ordering.ImplementationPlanMilestones
(
    Id int IDENTITY(1, 1) NOT NULL,
    ImplementationPlanId int NULL DEFAULT NULL,
    ContractBillingItemId int NULL DEFAULT NULL,
    [Order] int NOT NULL,
    Title nvarchar(1000) NOT NULL,
    [PaymentTrigger] nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ImplementationPlanMilestones PRIMARY KEY (Id),
    CONSTRAINT FK_ImplementationPlanMilestones_ImplementationPlan FOREIGN KEY (ImplementationPlanId) REFERENCES ordering.ImplementationPlans(Id),
    CONSTRAINT FK_ImplementationPlanMilestones_ContractBillingItem FOREIGN KEY (ContractBillingItemId) REFERENCES ordering.ContractBillingItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_ImplementationPlanMilestones_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ImplementationPlanMilestones_History));
