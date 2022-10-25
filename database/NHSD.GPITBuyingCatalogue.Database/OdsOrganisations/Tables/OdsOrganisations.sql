CREATE TABLE [ods_organisations].[OdsOrganisations]
(
    [Id]                    INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
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
    [IsActive]              BIT           NOT NULL DEFAULT (1),
)
