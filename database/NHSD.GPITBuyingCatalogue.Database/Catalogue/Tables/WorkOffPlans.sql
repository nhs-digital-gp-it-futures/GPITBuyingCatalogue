CREATE TABLE [catalogue].[WorkOffPlans]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    [SolutionId] NVARCHAR(14) NOT NULL,
    [StandardId] NVARCHAR(5) NOT NULL,
    [Details] NVARCHAR(300) NOT NULL,
    [CompletionDate] DATETIME2(7) NOT NULL,
    LastUpdated DATETIME2(7) CONSTRAINT DF_WorkOffPlans_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_WorkOffPlans PRIMARY KEY (Id),
    CONSTRAINT FK_WorkOffPlans_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_WorkOffPlans_Standard FOREIGN KEY (StandardId) REFERENCES catalogue.Standards(Id),
    CONSTRAINT FK_WorkOffPlans_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.WorkOffPlans_History));
