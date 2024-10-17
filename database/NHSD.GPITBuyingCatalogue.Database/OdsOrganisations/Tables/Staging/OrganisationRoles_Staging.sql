CREATE TABLE [ods_organisations].[OrganisationRoles_Staging]
(
    [Id]                INT             NOT NULL,
    [OrganisationId]    NVARCHAR(10)    NOT NULL,
    [RoleId]            NVARCHAR(10)    NOT NULL,
    [IsPrimaryRole]     BIT             NOT NULL DEFAULT(0),
)
