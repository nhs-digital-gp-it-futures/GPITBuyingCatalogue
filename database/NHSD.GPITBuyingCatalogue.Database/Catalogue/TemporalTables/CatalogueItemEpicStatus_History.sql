CREATE TABLE catalogue.CatalogueItemEpicStatus_History
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     IsMet bit NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0)NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_CatalogueItemEpicStatus_History
ON catalogue.CatalogueItemEpicStatus_History;
GO
