CREATE TABLE catalogue.MarketingContacts_History
(
     Id int NOT NULL,
     SolutionId nvarchar(14) NOT NULL,
     FirstName nvarchar(35) NULL,
     LastName nvarchar(35) NULL,
     Email nvarchar(255) NULL,
     PhoneNumber nvarchar(35) NULL,
     Department nvarchar(50) NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_MarketingContacts_History
ON catalogue.MarketingContacts_History;
GO

CREATE NONCLUSTERED INDEX IX_MarketingContacts_History_Id_Period_Columns
ON catalogue.MarketingContacts_History (SysEndTime, SysStartTime, Id);
GO
