CREATE TABLE dbo.CatalogueItemCapability
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_CatalogueItemCapability PRIMARY KEY CLUSTERED (CatalogueItemId, CapabilityId),
     CONSTRAINT FK_CatalogueItemCapability_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES dbo.CatalogueItem(CatalogueItemId) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemCapability_Capability FOREIGN KEY (CapabilityId) REFERENCES dbo.Capability(Id),
     CONSTRAINT FK_CatalogueItemCapability_Status FOREIGN KEY (StatusId) REFERENCES dbo.CatalogueItemCapabilityStatus(Id),
);
