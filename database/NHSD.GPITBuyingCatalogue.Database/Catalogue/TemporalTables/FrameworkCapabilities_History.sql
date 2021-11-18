﻿CREATE TABLE catalogue.FrameworkCapabilities_History
(
     FrameworkId nvarchar(10) NOT NULL,
     CapabilityId int NOT NULL,
     IsFoundation bit NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
