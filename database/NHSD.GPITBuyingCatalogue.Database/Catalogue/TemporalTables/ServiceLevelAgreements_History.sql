CREATE TABLE catalogue.ServiceLevelAgreements_History
(
    SolutionId nvarchar(14) NOT NULL,
    SlaType INT NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
