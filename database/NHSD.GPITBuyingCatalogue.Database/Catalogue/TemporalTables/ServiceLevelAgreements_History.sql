CREATE TABLE catalogue.ServiceLevelAgreements_History
(
    SolutionId nvarchar(14) NOT NULL,
    SlaType INT NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_ServiceLevelAgreements_History
ON catalogue.ServiceLevelAgreements_History;
GO

CREATE NONCLUSTERED INDEX IX_ServiceLevelAgreements_History_SolutionId_Period_Columns
ON catalogue.ServiceLevelAgreements_History (SysEndTime, SysStartTime, SolutionId);
GO
