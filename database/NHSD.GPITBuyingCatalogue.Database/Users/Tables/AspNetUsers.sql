﻿CREATE TABLE users.AspNetUsers
(
     Id uniqueidentifier NOT NULL,
     UserName nvarchar(256) NOT NULL,
     NormalizedUserName nvarchar(256) NOT NULL,
     Email nvarchar(256) NOT NULL,
     NormalizedEmail nvarchar(256) NOT NULL,
     EmailConfirmed bit CONSTRAINT DF_AspNetUsers_EmailConfirmed DEFAULT 0 NOT NULL,
     PasswordHash nvarchar(max) NULL,
     SecurityStamp nvarchar(max) NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     PhoneNumber nvarchar(35) NULL,
     PhoneNumberConfirmed bit CONSTRAINT DF_AspNetUsers_PhoneNumberConfirmed DEFAULT 0 NOT NULL,
     TwoFactorEnabled bit NOT NULL,
     LockoutEnd datetimeoffset(7) NULL,
     LockoutEnabled bit NOT NULL,
     AccessFailedCount int NOT NULL,
     PrimaryOrganisationId uniqueidentifier NOT NULL, 
     OrganisationFunction nvarchar(50) NOT NULL, 
     [Disabled] bit NOT NULL,
     CatalogueAgreementSigned bit CONSTRAINT DF_AspNetUsers_CatalogueAgreementSigned DEFAULT 0 NOT NULL,
     FirstName nvarchar(100) NOT NULL, 
     LastName nvarchar(100) NOT NULL, 
     CONSTRAINT PK_AspNetUsers PRIMARY KEY NONCLUSTERED (Id),
     CONSTRAINT AK_AspNetUsers_NormalizedUserName UNIQUE CLUSTERED (NormalizedUserName),
     CONSTRAINT AK_AspNetUsers_NormalizedEmail UNIQUE NONCLUSTERED (NormalizedEmail),
);
