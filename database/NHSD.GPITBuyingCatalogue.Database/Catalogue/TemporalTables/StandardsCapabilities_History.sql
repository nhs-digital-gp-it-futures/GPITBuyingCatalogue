﻿CREATE TABLE catalogue.StandardsCapabilities_History
(
    StandardId NVARCHAR(5) NOT NULL,
    CapabilityId INT NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
