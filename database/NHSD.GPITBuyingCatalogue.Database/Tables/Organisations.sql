CREATE TABLE dbo.Organisations
(
	OrganisationId uniqueidentifier NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Address] nvarchar(max) NULL,
    OdsCode nvarchar(8) NULL,
    PrimaryRoleId nvarchar(8) NULL,
    CatalogueAgreementSigned bit DEFAULT 0 NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_Organisations PRIMARY KEY NONCLUSTERED (OrganisationId),
    INDEX IX_OrganisationName CLUSTERED ([Name])
);
