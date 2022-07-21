CREATE TABLE ordering.DataProcessingPlanCategories_History
(
    Id int NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
