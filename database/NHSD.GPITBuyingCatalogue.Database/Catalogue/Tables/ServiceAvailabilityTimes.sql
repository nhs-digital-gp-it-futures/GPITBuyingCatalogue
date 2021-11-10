CREATE TABLE [catalogue].[ServiceAvailabilityTimes]
(
    [Id] INT IDENTITY(1,1) NOT NULL,
    SolutionId nvarchar(14) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    TimeFrom datetime2(7) NOT NULL,
    TimeUntil datetime2(7) NOT NULL,
    ApplicableDays nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) CONSTRAINT DF_ServiceAvailabilityTimes_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
	SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PKaServiceAvailabilityTimes PRIMARY KEY (Id),
    CONSTRAINT FK_ServiceAvailabilityTimes_ServiceLevelAgreements FOREIGN KEY (SolutionId) REFERENCES catalogue.ServiceLevelAgreements(SolutionId) ON DELETE CASCADE,
    CONSTRAINT FK_ServiceAvailabilityTimes_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
)
