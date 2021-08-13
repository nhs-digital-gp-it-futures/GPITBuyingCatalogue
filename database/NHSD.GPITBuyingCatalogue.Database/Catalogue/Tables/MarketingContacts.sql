CREATE TABLE catalogue.MarketingContacts
(
     Id int IDENTITY(1, 1) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     FirstName nvarchar(35) NULL,
     LastName nvarchar(35) NULL,
     Email nvarchar(255) NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     CONSTRAINT PK_MarketingContacts PRIMARY KEY (Id),
     CONSTRAINT FK_MarketingContacts_Solution FOREIGN KEY (SolutionId) REFERENCES catalogue.Solutions(CatalogueItemId) ON DELETE CASCADE,
     CONSTRAINT FK_MarketingContacts_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
