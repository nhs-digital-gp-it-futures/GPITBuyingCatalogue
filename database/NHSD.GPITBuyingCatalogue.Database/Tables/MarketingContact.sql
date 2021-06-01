CREATE TABLE dbo.MarketingContact
(
     Id int IDENTITY(1, 1) NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     FirstName nvarchar(35) NULL,
     LastName nvarchar(35) NULL,
     Email nvarchar(255) NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy uniqueidentifier NOT NULL,
     CONSTRAINT PK_MarketingContact PRIMARY KEY CLUSTERED (SolutionId, Id),
     CONSTRAINT FK_MarketingContact_Solution FOREIGN KEY (SolutionId) REFERENCES dbo.Solution(Id) ON DELETE CASCADE
);
