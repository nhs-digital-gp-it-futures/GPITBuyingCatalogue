CREATE TABLE catalogue.CatalogueItemEpics
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId int NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_CatalogueItemEpics PRIMARY KEY (CatalogueItemId, CapabilityId, EpicId),
     CONSTRAINT FK_CatalogueItemEpics_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_CatalogueItemEpics_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
     CONSTRAINT FK_CatalogueItemEpics_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemEpics_Status FOREIGN KEY (StatusId) REFERENCES catalogue.CatalogueItemEpicStatus(Id),
     CONSTRAINT FK_CatalogueItemEpics_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.CatalogueItemEpics_History));
