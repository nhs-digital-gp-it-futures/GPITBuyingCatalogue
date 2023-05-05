CREATE TABLE catalogue.FilterEpics
(
     FilterId int NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_FilterEpics PRIMARY KEY (FilterId, EpicId),
     CONSTRAINT FK_FilterEpics_Filter FOREIGN KEY (FilterId) REFERENCES catalogue.Filters(Id),
     CONSTRAINT FK_FilterEpics_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
     CONSTRAINT FK_FilterEpics_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.FilterEpics_History));
