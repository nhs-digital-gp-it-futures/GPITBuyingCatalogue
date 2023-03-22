CREATE TABLE organisations.Organisations_History
(
    Id INT NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Address] nvarchar(max) NULL,
    OdsCode nvarchar(8) NULL,
    PrimaryRoleId nvarchar(8) NULL,
    OrganisationRoleId nvarchar(8) NULL,
    CatalogueAgreementSigned bit NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL,
    [ExternalIdentifier] NVARCHAR(100) NULL, 
    [InternalIdentifier] NVARCHAR(103) NULL,
    [OrganisationTypeId] INT NULL
);
