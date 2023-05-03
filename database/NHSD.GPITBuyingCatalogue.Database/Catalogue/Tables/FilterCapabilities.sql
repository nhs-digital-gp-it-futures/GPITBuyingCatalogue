CREATE TABLE catalogue.FilterCapabilities
(
     FilterId nvarchar(10) NOT NULL,
     CapabilityId int NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_FilterCapabilities PRIMARY KEY (FilterId, CapabilityId),
     CONSTRAINT FK_FilterCapabilities_Filter FOREIGN KEY (FilterId) REFERENCES catalogue.Filters(Id),
     CONSTRAINT FK_FilterCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_FilterCapabilities_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.FilterCapabilities_History));
