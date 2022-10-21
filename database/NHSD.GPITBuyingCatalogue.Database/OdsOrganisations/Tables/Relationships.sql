CREATE TABLE [ods_organisations].[Relationships]
(
    [UniqueRelId]       INT NOT NULL PRIMARY KEY,
    [RelTypeId]         INT NOT NULL,
    [TargetRoleId]      INT NOT NULL,
    [OrganisationId]    INT NOT NULL,
    CONSTRAINT FK_Relationships_RelType         FOREIGN KEY (RelTypeId)         REFERENCES ods_organisations.RelationshipTypes (RelTypeId),
    CONSTRAINT FK_Relationships_Organisation    FOREIGN KEY (OrganisationId)    REFERENCES ods_organisations.OdsOrganisations (OrganisationId),
    CONSTRAINT FK_Relationships_TargetRole      FOREIGN KEY (TargetRoleId)      REFERENCES ods_organisations.RoleTypes (RoleId),
)
