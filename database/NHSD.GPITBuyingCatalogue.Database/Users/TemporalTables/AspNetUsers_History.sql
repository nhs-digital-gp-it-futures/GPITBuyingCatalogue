CREATE TABLE users.AspNetUsers_History
(
     Id int NOT NULL,
     UserName nvarchar(256) NOT NULL,
     NormalizedUserName nvarchar(256) NOT NULL,
     Email nvarchar(256) NOT NULL,
     NormalizedEmail nvarchar(256) NOT NULL,
     EmailConfirmed bit NOT NULL,
     PasswordHash nvarchar(max) NULL,
     SecurityStamp nvarchar(max) NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     PhoneNumber nvarchar(35) NULL,
     PhoneNumberConfirmed bit NOT NULL,
     TwoFactorEnabled bit NOT NULL,
     LockoutEnd datetimeoffset(7) NULL,
     LockoutEnabled bit NOT NULL,
     AccessFailedCount int NOT NULL,
     PrimaryOrganisationId int NOT NULL, 
     OrganisationFunction nvarchar(50) NOT NULL, 
     [Disabled] bit NOT NULL,
     CatalogueAgreementSigned bit NOT NULL,
     FirstName nvarchar(100) NOT NULL, 
     LastName nvarchar(100) NOT NULL,
     LastUpdated datetime2(7) NOT NULL,
     LastUpdatedBy int NULL,
     SysStartTime datetime2(0) NOT NULL,
     SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_AspNetUsers_History
ON users.AspNetUsers_History;
GO

CREATE NONCLUSTERED INDEX IX_AspNetUsers_History_Id_Period_Columns
ON users.AspNetUsers_History (SysEndTime, SysStartTime, Id);
GO
