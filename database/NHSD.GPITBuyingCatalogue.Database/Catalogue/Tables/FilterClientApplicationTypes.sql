CREATE TABLE catalogue.FilterClientApplicationTypes
(
     FilterId nvarchar(10) NOT NULL,
     ClientApplicationTypeId int NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_FilterClientApplicationTypes PRIMARY KEY (FilterId, ClientApplicationTypeId),
     CONSTRAINT FK_FilterClientApplicationTypes_Filter FOREIGN KEY (FilterId) REFERENCES catalogue.Filters(Id),
     CONSTRAINT FK_FilterClientApplicationTypes_ClientApplicationType FOREIGN KEY (ClientApplicationTypeId) REFERENCES catalogue.ClientApplicationTypes(Id),
     CONSTRAINT FK_FilterClientApplicationTypes_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.FilterClientApplicationTypes_History));
