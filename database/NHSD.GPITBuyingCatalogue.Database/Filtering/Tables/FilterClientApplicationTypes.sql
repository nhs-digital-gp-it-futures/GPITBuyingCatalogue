CREATE TABLE filtering.FilterClientApplicationTypes
(
     Id int IDENTITY(1, 1) NOT NULL,
     FilterId int NOT NULL,
     ClientApplicationTypeId int NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_FilterClientApplicationTypes PRIMARY KEY (Id),
     CONSTRAINT FK_FilterClientApplicationTypes_Filter FOREIGN KEY (FilterId) REFERENCES filtering.Filters(Id),
     CONSTRAINT FK_FilterClientApplicationTypes_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = filtering.FilterClientApplicationTypes_History));
