CREATE TABLE [ods_organisations].[OdsOrganisations]
(
    [OrganisationId]        INT           NOT NULL PRIMARY KEY,
    [Name]                  NVARCHAR(255) NOT NULL,
    [AddressLine1]          NVARCHAR(255) NOT NULL,
    [AddressLine2]          NVARCHAR(255) NOT NULL,
    [AddressLine3]          NVARCHAR(255) NOT NULL,
    [Town]                  NVARCHAR(100) NOT NULL,
    [County]                NVARCHAR(100) NOT NULL,
    [Status]                NVARCHAR(100) NOT NULL,
    [UniqueRoleId]          INT           NOT NULL,
    [RoleId]                INT           NOT NULL,
    CONSTRAINT FK_OdsOrganisations_UniqueRole FOREIGN KEY (UniqueRoleId)    REFERENCES [ods_organisations].[Roles] (UniqueRoleId),
    CONSTRAINT FK_OdsOrganisations_Role       FOREIGN KEY (RoleId)          REFERENCES [ods_organisations].[RoleTypes] (RoleId)
)
