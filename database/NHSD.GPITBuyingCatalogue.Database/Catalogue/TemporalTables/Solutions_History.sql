CREATE TABLE catalogue.Solutions_History
(
     CatalogueItemId nvarchar(14) NOT NULL,
     Summary nvarchar(350) NULL,
     FullDescription nvarchar(3000) NULL,
     Features nvarchar(max) NULL,
     ClientApplication nvarchar(max) NULL,
     Hosting nvarchar(max) NULL,
     ImplementationDetail nvarchar(1100) NULL,
     RoadMap nvarchar(1000) NULL,
     Integrations nvarchar(max) NULL,
     IntegrationsUrl nvarchar(1000) NULL,
     AboutUrl nvarchar(1000) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_Solutions_History
ON catalogue.Solutions_History;
GO

CREATE NONCLUSTERED INDEX IX_Solutions_History_CatalogueItemId_Period_Columns
ON catalogue.Solutions_History (SysEndTime, SysStartTime, CatalogueItemId);
GO
