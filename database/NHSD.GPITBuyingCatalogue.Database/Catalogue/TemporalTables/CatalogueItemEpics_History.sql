CREATE TABLE catalogue.CatalogueItemEpics_History
(
     CatalogueItemId nvarchar(14) NOT NULL,
     CapabilityId int NOT NULL,
     EpicId nvarchar(10) NOT NULL,
     StatusId int NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CatalogueItemEpics_History
ON catalogue.CatalogueItemEpics_History;
GO

CREATE NONCLUSTERED INDEX IX_CatalogueItemEpics_History_CatalogueItemId_CapabilityId_EpicId_Period_Columns
ON catalogue.CatalogueItemEpics_History (SysEndTime, SysStartTime, CatalogueItemId, CapabilityId, EpicId);
GO
