﻿CREATE TABLE users.AspNetUsers
(
     Id int IDENTITY(1, 1) NOT NULL,
     UserName nvarchar(256) NOT NULL,
     NormalizedUserName nvarchar(256) NOT NULL,
     Email nvarchar(256) NOT NULL,
     NormalizedEmail nvarchar(256) NOT NULL,
     EmailConfirmed bit CONSTRAINT DF_AspNetUsers_EmailConfirmed DEFAULT 0 NOT NULL,
     PasswordHash nvarchar(max) NULL,
     PasswordUpdatedDate datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     SecurityStamp nvarchar(max) NULL,
     ConcurrencyStamp nvarchar(max) NULL,
     PhoneNumber nvarchar(35) NULL,
     PhoneNumberConfirmed bit CONSTRAINT DF_AspNetUsers_PhoneNumberConfirmed DEFAULT 0 NOT NULL,
     TwoFactorEnabled bit NOT NULL,
     LockoutEnd datetimeoffset(7) NULL,
     LockoutEnabled bit NOT NULL,
     AccessFailedCount int NOT NULL,
     PrimaryOrganisationId int NOT NULL,
     OrganisationFunction nvarchar(50) NULL,
     [Disabled] bit NOT NULL,
     CatalogueAgreementSigned bit CONSTRAINT DF_AspNetUsers_CatalogueAgreementSigned DEFAULT 0 NOT NULL,
     FirstName nvarchar(100) NOT NULL,
     LastName nvarchar(100) NOT NULL,
     LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
     LastUpdatedBy int NULL,
     HasOptedInUserResearch BIT DEFAULT 0 NOT NULL,
     AcceptedTermsOfUseDate DATETIME2(7) NULL,
     SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
     SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
     PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
     CONSTRAINT PK_AspNetUsers PRIMARY KEY (Id),
     CONSTRAINT AK_AspNetUsers_NormalizedUserName UNIQUE (NormalizedUserName),
     CONSTRAINT AK_AspNetUsers_NormalizedEmail UNIQUE (NormalizedEmail),
     CONSTRAINT FK_AspNetUsers_OrganisationId FOREIGN KEY (PrimaryOrganisationId) REFERENCES organisations.Organisations (Id),
     CONSTRAINT FK_AspNetUsers_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = users.AspNetUsers_History));
