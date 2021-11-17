CREATE TABLE catalogue.ServiceLevelAgreements
(
    SolutionId nvarchar(14) NOT NULL,
    SlaType INT NOT NULL,
    LastUpdated datetime2(7) CONSTRAINT DF_ServiceLevelAgreement_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_ServiceLevelAgreements PRIMARY KEY (SolutionId),
    CONSTRAINT FK_ServiceLevelAgreements_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_ServiceLevelAgreements_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.ServiceLevelAgreements_History));
