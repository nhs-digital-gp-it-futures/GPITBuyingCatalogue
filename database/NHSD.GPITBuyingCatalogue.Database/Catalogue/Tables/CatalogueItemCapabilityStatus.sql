CREATE TABLE catalogue.CatalogueItemCapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     Pass bit NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_CatalogueItemCapabilityStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CatalogueItemCapabilityStatus_Name UNIQUE ([Name]),
     CONSTRAINT FK_CatalogueItemCapabilityStatus_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
