CREATE TABLE catalogue.CatalogueItemEpics
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId int NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_CatalogueItemEpic PRIMARY KEY CLUSTERED (CatalogueItemId, CapabilityId, EpicId),
     CONSTRAINT FK_CatalogueItemEpic_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_CatalogueItemEpic_Epic FOREIGN KEY (EpicId) REFERENCES catalogue.Epics(Id),
     CONSTRAINT FK_CatalogueItemEpic_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemEpic_Status FOREIGN KEY (StatusId) REFERENCES catalogue.CatalogueItemEpicStatus(Id),
);
