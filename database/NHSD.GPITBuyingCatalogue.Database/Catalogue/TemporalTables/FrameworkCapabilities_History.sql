CREATE TABLE catalogue.FrameworkCapabilities_History
(
     FrameworkId nvarchar(10) NOT NULL,
     CapabilityId int NOT NULL,
     IsFoundation bit NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_FrameworkCapabilities_History
ON catalogue.FrameworkCapabilities_History;
GO 

CREATE NONCLUSTERED INDEX IX_FrameworkCapabilities_History_FrameworkId_CapabilityId_Period_Columns
ON catalogue.FrameworkCapabilities_History (SysEndTime, SysStartTime, CapabilityId, FrameworkId);
GO
