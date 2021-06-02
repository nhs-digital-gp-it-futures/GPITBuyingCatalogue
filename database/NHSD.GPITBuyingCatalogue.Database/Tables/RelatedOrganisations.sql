CREATE TABLE dbo.RelatedOrganisations
(
    OrganisationId uniqueidentifier NOT NULL,
    RelatedOrganisationId uniqueidentifier NOT NULL,
    CONSTRAINT PK_RelatedOrganisations PRIMARY KEY (OrganisationId, RelatedOrganisationId),
    CONSTRAINT FK_RelatedOrganisations_OrganisationId FOREIGN KEY (OrganisationId) REFERENCES dbo.Organisations (OrganisationId),
    CONSTRAINT FK_RelatedOrganisations_RelatedOrganisationId FOREIGN KEY (RelatedOrganisationId) REFERENCES dbo.Organisations (OrganisationId),
);
