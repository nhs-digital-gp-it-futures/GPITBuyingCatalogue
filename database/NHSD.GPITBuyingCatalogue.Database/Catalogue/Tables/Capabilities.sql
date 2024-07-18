CREATE TABLE catalogue.Capabilities
(
     Id int NOT NULL,
     CapabilityRef AS 'C' + CAST(Id AS nvarchar(3)),
     [Version] nvarchar(10) NULL, --TODO Remove column in future release
     StatusId int NOT NULL,
     [Name] nvarchar(255) NOT NULL,
     [Description] nvarchar(500) NOT NULL,
     SourceUrl nvarchar(1000) NULL,
     EffectiveDate date NOT NULL,
     CategoryId int NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_Capabilities PRIMARY KEY (Id),
     CONSTRAINT FK_Capabilities_CapabilityCategory FOREIGN KEY (CategoryId) REFERENCES catalogue.CapabilityCategories(Id),
     CONSTRAINT FK_Capabilities_CapabilityStatus FOREIGN KEY (StatusId) REFERENCES catalogue.CapabilityStatus(Id),
     CONSTRAINT FK_Capabilities_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
     INDEX IX_Capabilities_CapabilityRef (CapabilityRef),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Capabilities_History));
