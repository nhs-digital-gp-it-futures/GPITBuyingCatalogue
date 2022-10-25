CREATE TABLE [ods_organisations].[OrganisationRelationships]
(
    [Id]                        INT             NOT NULL PRIMARY KEY,
    [RelTypeId]                 NVARCHAR(10)    NOT NULL,
    [TargetOrganisationId]      INT             NOT NULL,
    [OwnerOrganisationId]       INT             NOT NULL,
    CONSTRAINT FK_Relationships_RelType         FOREIGN KEY (RelTypeId)                 REFERENCES ods_organisations.RelationshipTypes (Id),
    CONSTRAINT FK_Relationships_Organisation    FOREIGN KEY (OwnerOrganisationId)       REFERENCES ods_organisations.OdsOrganisations (Id),
    CONSTRAINT FK_Relationships_TargetRole      FOREIGN KEY (TargetOrganisationId)      REFERENCES ods_organisations.OdsOrganisations (Id),
)
