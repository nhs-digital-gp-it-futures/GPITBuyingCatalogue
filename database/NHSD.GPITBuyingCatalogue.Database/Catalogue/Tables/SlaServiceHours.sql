CREATE TABLE [catalogue].[SlaServiceHours]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
    SolutionId nvarchar(14) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    TimeFrom datetime2(7) NOT NULL,
    TimeUntil datetime2(7) NOT NULL,
    ApplicableDays nvarchar(1000) NOT NULL,
    LastUpdated datetime2(7) CONSTRAINT DF_SlaServiceAvailabilityTimes_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    CONSTRAINT PK_SlaServiceAvailabilityTimes PRIMARY KEY (Id),
    CONSTRAINT FK_SlaServiceAvailabilityTimes_ServiceLevelAgreements FOREIGN KEY (SolutionId) REFERENCES catalogue.ServiceLevelAgreements(SolutionId) ON DELETE CASCADE,
    CONSTRAINT FK_SlaServiceAvailabilityTimes_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
)
