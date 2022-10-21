CREATE TABLE [ods_organisations].[OrganisationRoles]
(
    [UniqueRoleId]      INT             NOT NULL PRIMARY KEY,
    [OrganisationId]    INT             NOT NULL,
    [RoleId]            NVARCHAR(10)    NOT NULL,
    [IsPrimaryRole]     BIT             NOT NULL DEFAULT(0),
    CONSTRAINT FK_Roles_Organisation    FOREIGN KEY (OrganisationId)    REFERENCES [ods_organisations].[OdsOrganisations] (OrganisationId),
    CONSTRAINT FK_Roles_Role            FOREIGN KEY (RoleId)            REFERENCES [ods_organisations].[RoleTypes] (RoleId),
)
