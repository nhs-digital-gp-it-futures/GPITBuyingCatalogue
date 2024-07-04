CREATE TABLE catalogue.Solutions
(
     CatalogueItemId nvarchar(14) NOT NULL,
     Summary nvarchar(350) NULL,
     FullDescription nvarchar(3000) NULL,
     Features nvarchar(max) NULL,
     ClientApplication nvarchar(max) NULL,
     Hosting nvarchar(max) NULL,
     ImplementationDetail nvarchar(1100) NULL,
     RoadMap nvarchar(1000) NULL,
     Integrations nvarchar(max) NULL, -- TODO: Remove in next release
     IntegrationsUrl nvarchar(1000) NULL,
     AboutUrl nvarchar(1000) NULL,
     IsPilotSolution bit DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_Solutions PRIMARY KEY (CatalogueItemId),
     CONSTRAINT FK_Solutions_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
     CONSTRAINT FK_Solutions_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Solutions_History));
