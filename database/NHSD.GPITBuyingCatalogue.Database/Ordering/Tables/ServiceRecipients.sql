CREATE TABLE ordering.ServiceRecipients
(
    OdsCode nvarchar(8) NOT NULL,
    [Name] nvarchar(256) NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ServiceRecipients PRIMARY KEY (OdsCode),
);
