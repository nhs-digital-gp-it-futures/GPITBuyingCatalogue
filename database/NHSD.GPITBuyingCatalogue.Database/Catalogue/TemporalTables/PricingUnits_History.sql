CREATE TABLE catalogue.PricingUnits_History
(
    Id smallint NOT NULL,
    [Name] nvarchar(20) NULL,
    TierName nvarchar(30) NULL,
    [RangeDescription] nvarchar(100) NULL,
    [Description] nvarchar(100) NOT NULL,
    [Definition] nvarchar(1000) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
