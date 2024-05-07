﻿CREATE TABLE catalogue.Frameworks
(
    Id NVARCHAR(36) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    ShortName NVARCHAR(100) NULL,
    [Description] NVARCHAR(max) NULL,
    [Owner] NVARCHAR(100) NULL,
    ActiveDate DATE NULL,
    ExpiryDate DATE NULL,
    LastUpdated DATETIME2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy INT NULL,
    SysStartTime DATETIME2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    LocalFundingOnly BIT NULL,
    FundingTypes NVARCHAR(30) NULL,
    IsExpired BIT NOT NULL DEFAULT(0),
    SupportsFoundationSolution BIT NULL DEFAULT(0),
    MaximumTerm INT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Frameworks PRIMARY KEY (Id),
    CONSTRAINT FK_Frameworks_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers (Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.Frameworks_History));
