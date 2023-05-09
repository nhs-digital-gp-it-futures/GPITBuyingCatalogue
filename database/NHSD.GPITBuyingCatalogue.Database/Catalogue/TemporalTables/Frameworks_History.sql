CREATE TABLE catalogue.Frameworks_History
(
    Id nvarchar(36) NOT NULL,
    [Name] nvarchar(100) NULL,
    ShortName nvarchar(25) NOT NULL DEFAULT(''),
    [Description] nvarchar(max) NULL,
    [Owner] nvarchar(100) NULL,
    ActiveDate date NULL,
    ExpiryDate date NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
    LocalFundingOnly BIT NULL,
    IsExpired BIT NOT NULL DEFAULT(0),
);
