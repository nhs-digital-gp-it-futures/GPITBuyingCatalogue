CREATE TABLE catalogue.CatalogueItemEpicStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     IsMet bit NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_CatalogueItemEpicStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CatalogueItemEpicStatus_Name UNIQUE ([Name]),
     CONSTRAINT FK_CatalogueItemEpicStatus_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CatalogueItemEpicStatus_History));
