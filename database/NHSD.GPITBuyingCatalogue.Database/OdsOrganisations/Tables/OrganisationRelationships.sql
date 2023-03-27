CREATE TABLE [ods_organisations].[OrganisationRelationships]
(
    [Id]                        INT                     NOT NULL PRIMARY KEY,
    [RelationshipTypeId]        NVARCHAR(10)            NOT NULL,
    [TargetOrganisationId]      NVARCHAR(10)            NOT NULL,
    [OwnerOrganisationId]       NVARCHAR(10)            NOT NULL,
    CONSTRAINT FK_Relationships_Relationship            FOREIGN KEY (RelationshipTypeId)        REFERENCES ods_organisations.RelationshipTypes (Id),
    CONSTRAINT FK_Relationships_OwnerOrganisation       FOREIGN KEY (OwnerOrganisationId)       REFERENCES ods_organisations.OdsOrganisations (Id),
    CONSTRAINT FK_Relationships_TargetOrganisation      FOREIGN KEY (TargetOrganisationId)      REFERENCES ods_organisations.OdsOrganisations (Id),
)

GO
CREATE NONCLUSTERED INDEX IX_RelationshipType_TargetOwnerOrganisationId ON [ods_organisations].[OrganisationRelationships] ([RelationshipTypeId], [OwnerOrganisationId], [TargetOrganisationId])
