CREATE TABLE [catalogue].[SlaServiceLevels]
(
	Id INT IDENTITY(1,1) NOT NULl,
    SolutionId NVARCHAR(14) NOT NULL,
    TypeOfService NVARCHAR(100) NOT NULL,
    ServiceLevel NVARCHAR(1000) NOT NULL,
    HowMeasured NVARCHAR(1000) NOT NULL,
    ServiceCredits BIT NOT NULL,
    LastUpdated datetime2(7) CONSTRAINT DF_SlaServiceLevels_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    CONSTRAINT PK_SlaServiceLevels PRIMARY KEY (Id),
    CONSTRAINT FK_SlaServiceLevels_ServiceLevelAgreements FOREIGN KEY (SolutionId) REFERENCES catalogue.ServiceLevelAgreements(SolutionId) ON DELETE CASCADE,
    CONSTRAINT FK_SlaServiceLevels_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
)
