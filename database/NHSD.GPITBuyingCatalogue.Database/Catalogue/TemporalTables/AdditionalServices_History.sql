CREATE TABLE catalogue.AdditionalServices_History
(
    CatalogueItemId nvarchar(14) NOT NULL,
    Summary nvarchar(300) NULL,
    FullDescription nvarchar(3000) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SolutionId nvarchar(14) NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
