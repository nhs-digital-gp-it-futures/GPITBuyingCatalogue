﻿CREATE TABLE [ods_organisations].[OdsOrganisations]
(
    [OrganisationId]        INT           NOT NULL PRIMARY KEY,
    [Name]                  NVARCHAR(255) NOT NULL,
    [AddressLine1]          NVARCHAR(255) NULL,
    [AddressLine2]          NVARCHAR(255) NULL,
    [AddressLine3]          NVARCHAR(255) NULL,
    [AddressLine4]          NVARCHAR(255) NULL,
    [AddressLine5]          NVARCHAR(255) NULL,
    [Town]                  NVARCHAR(100) NULL,
    [County]                NVARCHAR(100) NULL,
    [Postcode]              NVARCHAR(10)  NULL,
    [Country]               NVARCHAR(100) NULL,
    [Status]                NVARCHAR(100) NOT NULL,
)
