CREATE TABLE [catalogue].[StandardsCapabilities]
(
    StandardId NVARCHAR(5) NOT NULL,
    CapabilityId INT NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_StandardsCapabilities PRIMARY KEY (StandardId, CapabilityId),
    CONSTRAINT FK_StandardsCapabilities_Standard FOREIGN KEY (StandardId) REFERENCES catalogue.Standards(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StandardsCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id) ON DELETE CASCADE,
    CONSTRAINT FK_StandardsCapabilities_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.StandardsCapabilities_History));
