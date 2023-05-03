CREATE TABLE catalogue.Filters_History
(
    Id nvarchar(10) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    OrganisationId int NOT NULL,
    FrameworkId NVARCHAR(10) NULL,
    Created datetime2(7) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastPublished datetime2(7) NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
