CREATE TABLE [catalogue].[CapabilityEpics]
(
    [CapabilityId] INT NOT NULL,
    [EpicId] NVARCHAR(10) NOT NULL,
    CompliancyLevelId int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_CapabilityEpic PRIMARY KEY (CapabilityId, EpicId),
    CONSTRAINT FK_CapabilityEpic_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities (Id),
    CONSTRAINT FK_CapabilityEpic_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics (Id),
    CONSTRAINT FK_CapabilityEpics_CompliancyLevel FOREIGN KEY (CompliancyLevelId) REFERENCES catalogue.CompliancyLevels(Id)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CapabilityEpics_History));
