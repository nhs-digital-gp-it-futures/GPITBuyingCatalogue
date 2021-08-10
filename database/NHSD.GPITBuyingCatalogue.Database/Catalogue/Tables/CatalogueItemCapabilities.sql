CREATE TABLE catalogue.CatalogueItemCapabilities
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId uniqueidentifier NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_CatalogueItemCapability PRIMARY KEY CLUSTERED (CatalogueItemId, CapabilityId),
     CONSTRAINT FK_CatalogueItemCapability_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemCapability_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_CatalogueItemCapability_Status FOREIGN KEY (StatusId) REFERENCES catalogue.CatalogueItemCapabilityStatus(Id),
);
