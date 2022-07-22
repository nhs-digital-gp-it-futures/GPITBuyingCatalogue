CREATE TABLE ordering.DataProcessingPlanSteps
(
    Id int IDENTITY(1, 1) NOT NULL,
    DataProcessingPlanId int NOT NULL,
    DataProcessingPlanCategoryId int NOT NULL,
    Details nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_DataProcessingPlanSteps PRIMARY KEY (Id),
    CONSTRAINT FK_DataProcessingPlanSteps_Plan FOREIGN KEY (DataProcessingPlanId) REFERENCES ordering.DataProcessingPlans(Id),
    CONSTRAINT FK_DataProcessingPlanSteps_Category FOREIGN KEY (DataProcessingPlanCategoryId) REFERENCES ordering.DataProcessingPlanCategories(Id),
    CONSTRAINT FK_DataProcessingPlanSteps_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.DataProcessingPlanSteps_History));
