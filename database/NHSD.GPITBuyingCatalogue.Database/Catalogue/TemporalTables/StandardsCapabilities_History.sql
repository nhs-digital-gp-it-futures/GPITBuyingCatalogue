﻿CREATE TABLE catalogue.StandardsCapabilities_History
(
    StandardId NVARCHAR(5) NOT NULL,
    CapabilityId INT NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_StandardsCapabilities_History
ON catalogue.StandardsCapabilities_History;
GO

CREATE NONCLUSTERED INDEX IX_StandardsCapabilities_History_StandardId_CapabilityId_Period_Columns
ON catalogue.StandardsCapabilities_History (SysEndTime, SysStartTime, StandardId, CapabilityId);
GO
