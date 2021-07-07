CREATE TABLE dbo.CatalogueItemEpic
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_CatalogueItemEpic PRIMARY KEY CLUSTERED (CatalogueItemId, CapabilityId, EpicId),
     CONSTRAINT FK_CatalogueItemEpic_Capability FOREIGN KEY (CapabilityId) REFERENCES dbo.Capability(Id),
     CONSTRAINT FK_CatalogueItemEpic_Epic FOREIGN KEY (EpicId) REFERENCES dbo.Epic(Id),
     CONSTRAINT FK_CatalogueItemEpic_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES dbo.CatalogueItem(CatalogueItemId) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemEpic_Status FOREIGN KEY (StatusId) REFERENCES dbo.CatalogueItemEpicStatus(Id)
);
