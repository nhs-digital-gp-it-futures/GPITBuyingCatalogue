CREATE TABLE [ods_organisations].[OrganisationRoles]
(
    [Id]                INT             NOT NULL PRIMARY KEY,
    [OrganisationId]    NVARCHAR(10)    NOT NULL,
    [RoleId]            NVARCHAR(10)    NOT NULL,
    [IsPrimaryRole]     BIT             NOT NULL DEFAULT(0),
    [IsActive]          BIT             NOT NULL DEFAULT (1),
    CONSTRAINT FK_Roles_Organisation    FOREIGN KEY (OrganisationId)    REFERENCES [ods_organisations].[OdsOrganisations] (Id),
    CONSTRAINT FK_Roles_Role            FOREIGN KEY (RoleId)            REFERENCES [ods_organisations].[RoleTypes] (Id),
)

GO
CREATE NONCLUSTERED INDEX IX_RoleId_OrganisationId          ON [ods_organisations].[OrganisationRoles] ([RoleId]) INCLUDE ([OrganisationId])
GO
CREATE NONCLUSTERED INDEX IX_IsPrimaryRole_OrganisationId   ON [ods_organisations].[OrganisationRoles] ([OrganisationId], [IsPrimaryRole])
GO
CREATE NONCLUSTERED INDEX IX_IsActive ON [ods_organisations].[OrganisationRoles] ([IsActive]) INCLUDE ([OrganisationId],[RoleId],[IsPrimaryRole])
