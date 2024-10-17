CREATE TABLE [ods_organisations].[OrganisationRelationships_Staging]
(
    [Id]                        INT                     NOT NULL,
    [RelationshipTypeId]        NVARCHAR(10)            NOT NULL,
    [TargetOrganisationId]      NVARCHAR(10)            NOT NULL,
    [OwnerOrganisationId]       NVARCHAR(10)            NOT NULL,
)
