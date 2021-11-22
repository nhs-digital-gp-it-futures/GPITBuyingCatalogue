CREATE TABLE catalogue.MarketingContacts
(
     Id int IDENTITY(1, 1) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     FirstName nvarchar(35) NULL,
     LastName nvarchar(35) NULL,
     Email nvarchar(255) NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) CONSTRAINT DF_MarketingContacts_LastUpdated DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_MarketingContacts PRIMARY KEY (Id),
     CONSTRAINT FK_MarketingContacts_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
     CONSTRAINT FK_MarketingContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.MarketingContacts_History));
