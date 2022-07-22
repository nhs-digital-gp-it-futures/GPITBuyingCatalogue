CREATE TABLE ordering.DataProcessingPlanSteps_History
(
    Id int NOT NULL,
    DataProcessingPlanId int NOT NULL,
    DataProcessingPlanCategoryId int NOT NULL,
    Details nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
);
