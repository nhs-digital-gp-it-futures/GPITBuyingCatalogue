CREATE TABLE [catalogue].[SlaContacts]
(
	 Id INT NOT NULL IDENTITY(1,1),
     SolutionId nvarchar(14) NOT NULL,
     Channel nvarchar(300) NOT NULL,
     ContactInformation nvarchar(1000) NOT NULL,
     TimeFrom datetime2(7) NOT NULL,
     TimeUntil datetime2(7) NOT NULL,
     LastUpdated datetime2(7) CONSTRAINT DF_SlaContacts_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     CONSTRAINT PK_SlaContacts PRIMARY KEY (Id),
     CONSTRAINT FK_SlaContacts_ServiceLevelAgreement FOREIGN KEY (SolutionId) REFERENCES catalogue.ServiceLevelAgreements(SolutionId),
     CONSTRAINT FK_SlaContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
