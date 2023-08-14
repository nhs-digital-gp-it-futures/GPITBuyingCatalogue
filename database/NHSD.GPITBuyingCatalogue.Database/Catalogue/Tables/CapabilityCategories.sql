CREATE TABLE catalogue.CapabilityCategories
(
     Id int IDENTITY(1, 1) NOT NULL,
     [Name] nvarchar(50) NOT NULL,
     [Description] nvarchar(200) NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_CapabilityCategories PRIMARY KEY (Id),
     CONSTRAINT AK_CapabilityCategories_Name UNIQUE ([Name]),
     CONSTRAINT FK_CapabilityCategories_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CapabilityCategories_History));
