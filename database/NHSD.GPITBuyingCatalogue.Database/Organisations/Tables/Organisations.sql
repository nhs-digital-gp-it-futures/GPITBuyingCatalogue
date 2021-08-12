CREATE TABLE organisations.Organisations
(
    Id int IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Address] nvarchar(max) NULL,
    OdsCode nvarchar(8) NULL,
    PrimaryRoleId nvarchar(8) NULL,
    CatalogueAgreementSigned bit DEFAULT 0 NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_Organisations PRIMARY KEY(Id),
    CONSTRAINT AK_Organisations_Name UNIQUE ([Name]),
);
