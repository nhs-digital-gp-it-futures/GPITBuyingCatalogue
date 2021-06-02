CREATE TABLE dbo.Solution
(
     Id nvarchar(14) NOT NULL,
     [Version] nvarchar(10) NULL,
     Summary nvarchar(350) NULL,
     FullDescription nvarchar(3000) NULL,
     Features nvarchar(max) NULL,
     ClientApplication nvarchar(max) NULL,
     Hosting nvarchar(max) NULL,
     ImplementationDetail nvarchar(1100) NULL,
     RoadMap nvarchar(1000) NULL,
     IntegrationsUrl nvarchar(1000) NULL,
     AboutUrl nvarchar(1000) NULL,
     ServiceLevelAgreement nvarchar(1000) NULL,
     WorkOfPlan nvarchar(max) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_Solution PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT FK_Solution_CatalogueItem FOREIGN KEY (Id) REFERENCES dbo.CatalogueItem(CatalogueItemId) ON DELETE CASCADE
);
