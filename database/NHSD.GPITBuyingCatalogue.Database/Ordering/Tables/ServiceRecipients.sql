CREATE TABLE ordering.ServiceRecipients
(
    OdsCode nvarchar(8) NOT NULL,
    [Name] nvarchar(256) NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ServiceRecipients PRIMARY KEY (OdsCode),
    CONSTRAINT FK_ServiceRecipients_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = ordering.ServiceRecipients_History));
