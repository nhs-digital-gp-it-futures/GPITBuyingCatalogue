CREATE TABLE ordering.ServiceRecipients_History
(
    OdsCode nvarchar(8) NOT NULL,
    [Name] nvarchar(256) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
