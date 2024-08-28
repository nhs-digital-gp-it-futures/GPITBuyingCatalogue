CREATE TABLE catalogue.ServiceAvailabilityTimes_History
(
    [Id] INT NOT NULL,
    SolutionId nvarchar(14) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    TimeFrom datetime2(7) NOT NULL,
    TimeUntil datetime2(7) NOT NULL,
    ApplicableDays nvarchar(1000) NULL,
    IncludedDays NVARCHAR(15) NULL,
    IncludesBankHolidays BIT DEFAULT(0) NOT NULL,
    AdditionalInformation NVARCHAR(500) NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
