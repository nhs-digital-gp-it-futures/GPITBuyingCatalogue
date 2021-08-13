CREATE TABLE catalogue.CatalogueItemCapabilities
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId int NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     CONSTRAINT PK_CatalogueItemCapabilities PRIMARY KEY CLUSTERED (CatalogueItemId, CapabilityId),
     CONSTRAINT FK_CatalogueItemCapabilities_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
     CONSTRAINT FK_CatalogueItemCapabilities_Capability FOREIGN KEY (CapabilityId) REFERENCES catalogue.Capabilities(Id),
     CONSTRAINT FK_CatalogueItemCapabilities_Status FOREIGN KEY (StatusId) REFERENCES catalogue.CatalogueItemCapabilityStatus(Id),
     CONSTRAINT FK_CatalogueItemCapabilities_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
