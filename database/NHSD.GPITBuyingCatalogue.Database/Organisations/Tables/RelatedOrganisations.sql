CREATE TABLE organisations.RelatedOrganisations
(
    OrganisationId int NOT NULL,
    RelatedOrganisationId int NOT NULL,
    CONSTRAINT PK_RelatedOrganisations PRIMARY KEY (OrganisationId, RelatedOrganisationId),
    CONSTRAINT FK_RelatedOrganisations_OrganisationId FOREIGN KEY (OrganisationId) REFERENCES organisations.Organisations (Id),
    CONSTRAINT FK_RelatedOrganisations_RelatedOrganisationId FOREIGN KEY (RelatedOrganisationId) REFERENCES organisations.Organisations (Id),
);
