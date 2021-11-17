CREATE TABLE catalogue.ServiceAvailabilityTimes_History
(
    [Id] INT NOT NULL,
    SolutionId nvarchar(14) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    TimeFrom datetime2(7) NOT NULL,
    TimeUntil datetime2(7) NOT NULL,
    ApplicableDays nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_ServiceAvailabilityTimes_History
ON catalogue.ServiceAvailabilityTimes_History;
GO
