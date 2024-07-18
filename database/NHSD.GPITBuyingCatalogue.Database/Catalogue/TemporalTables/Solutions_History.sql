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
     IntegrationsUrl nvarchar(1000) NULL,
     AboutUrl nvarchar(1000) NULL,
     IsPilotSolution bit DEFAULT 0 NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
