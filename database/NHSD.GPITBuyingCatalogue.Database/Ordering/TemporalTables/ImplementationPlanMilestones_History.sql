CREATE TABLE ordering.ImplementationPlanMilestones_History
(
    Id int NOT NULL,
    ImplementationPlanId int NULL,
    [Order] int NOT NULL,
    Title nvarchar(1000) NOT NULL,
    [PaymentTrigger] nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
