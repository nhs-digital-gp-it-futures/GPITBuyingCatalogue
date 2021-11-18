CREATE TABLE catalogue.SlaContacts_History
(
	 Id INT NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     Channel nvarchar(300) NOT NULL,
     ContactInformation nvarchar(1000) NOT NULL,
     TimeFrom datetime2(7) NOT NULL,
     TimeUntil datetime2(7) NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
