CREATE TABLE catalogue.FrameworkCapabilities
(
     FrameworkId nvarchar(36) NOT NULL,
     CapabilityId int NOT NULL,
     IsFoundation bit CONSTRAINT DF_FrameworkCapabilities_IsFoundation DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_FrameworkCapabilities PRIMARY KEY (FrameworkId, CapabilityId),
     CONSTRAINT FK_FrameworkCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_FrameworkCapabilities_Framework FOREIGN KEY (FrameworkId) REFERENCES catalogue.Frameworks(Id),
     CONSTRAINT FK_FrameworkCapabilities_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.FrameworkCapabilities_History));
