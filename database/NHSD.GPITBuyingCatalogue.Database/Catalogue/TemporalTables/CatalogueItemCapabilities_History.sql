CREATE TABLE catalogue.CatalogueItemCapabilities_History
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId int NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CatalogueItemCapabilities_History
ON catalogue.CatalogueItemCapabilities_History;
GO

CREATE NONCLUSTERED INDEX IX_CatalogueItemCapabilities_History_CatalogueItemId_CapabilityId_Period_Columns
ON catalogue.CatalogueItemCapabilities_History (SysEndTime, SysStartTime, CatalogueItemId, CapabilityId);
GO
