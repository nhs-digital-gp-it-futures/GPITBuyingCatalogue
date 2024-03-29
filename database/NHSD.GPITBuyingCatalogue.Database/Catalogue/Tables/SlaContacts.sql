﻿CREATE TABLE [catalogue].[SlaContacts]
(
	 Id INT NOT NULL IDENTITY(1,1),
     SolutionId nvarchar(14) NOT NULL,
     Channel nvarchar(300) NOT NULL,
     ContactInformation nvarchar(1000) NOT NULL,
     TimeFrom datetime2(7) NOT NULL,
     TimeUntil datetime2(7) NOT NULL,
     ApplicableDays nvarchar(1000) NULL DEFAULT NULL,
     LastUpdated datetime2(7) CONSTRAINT DF_SlaContacts_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_SlaContacts PRIMARY KEY (Id),
     CONSTRAINT FK_SlaContacts_ServiceLevelAgreement FOREIGN KEY (SolutionId) REFERENCES catalogue.ServiceLevelAgreements(SolutionId),
     CONSTRAINT FK_SlaContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.SlaContacts_History));
