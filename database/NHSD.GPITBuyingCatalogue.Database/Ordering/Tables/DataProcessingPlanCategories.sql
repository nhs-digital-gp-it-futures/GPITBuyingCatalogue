CREATE TABLE ordering.DataProcessingPlanCategories
(
    Id int IDENTITY(1, 1) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_DataProcessingPlanCategories PRIMARY KEY (Id),
    CONSTRAINT FK_DataProcessingPlanCategories_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.DataProcessingPlanCategories_History));
