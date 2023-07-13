CREATE TABLE catalogue.Epics
(
     Id nvarchar(10) NOT NULL,
     [Name] nvarchar(500) NOT NULL,
     [Description] nvarchar(1500) NULL,
     CapabilityId int NULL,
     SourceUrl nvarchar(max) NULL,
     CompliancyLevelId int NULL,
     IsActive bit CONSTRAINT DF_Epic_IsActive DEFAULT 0 NOT NULL,
     SupplierDefined bit CONSTRAINT DF_Epic_SupplierDefined DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_Epics PRIMARY KEY (Id),
     CONSTRAINT FK_Epics_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Epics_History));
