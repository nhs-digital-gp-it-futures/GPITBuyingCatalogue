CREATE TABLE organisations.RelatedOrganisations
(
    OrganisationId int NOT NULL,
    RelatedOrganisationId int NOT NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_RelatedOrganisations PRIMARY KEY (OrganisationId, RelatedOrganisationId),
    CONSTRAINT FK_RelatedOrganisations_OrganisationId FOREIGN KEY (OrganisationId) REFERENCES organisations.Organisations (Id),
    CONSTRAINT FK_RelatedOrganisations_RelatedOrganisationId FOREIGN KEY (RelatedOrganisationId) REFERENCES organisations.Organisations (Id),
);
