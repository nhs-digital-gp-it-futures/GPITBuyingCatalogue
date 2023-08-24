CREATE TABLE catalogue.Standards_History
(
    [Id] NVARCHAR(5) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [Url] NVARCHAR(1000) NOT NULL,
    [Version] NVARCHAR(10) NULL,
    StandardTypeId INT NOT NULL,
    IsDeleted BIT DEFAULT(0) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
