CREATE TABLE [ods_organisations].[OdsOrganisations_Staging]
(
    [Id]                    NVARCHAR(10)  NOT NULL,
    [Name]                  NVARCHAR(255) NOT NULL,
    [AddressLine1]          NVARCHAR(255) NULL,
    [AddressLine2]          NVARCHAR(255) NULL,
    [AddressLine3]          NVARCHAR(255) NULL,
    [Town]                  NVARCHAR(100) NULL,
    [County]                NVARCHAR(100) NULL,
    [Postcode]              NVARCHAR(10)  NULL,
    [Country]               NVARCHAR(100) NULL,
    [IsActive]              BIT           NOT NULL DEFAULT (1),
)
