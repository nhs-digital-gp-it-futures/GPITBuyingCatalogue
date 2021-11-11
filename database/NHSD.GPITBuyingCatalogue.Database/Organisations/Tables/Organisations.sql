CREATE TABLE organisations.Organisations
(
    Id int IDENTITY(1, 1) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    [Address] nvarchar(max) NULL,
    OdsCode nvarchar(8) NULL,
    PrimaryRoleId nvarchar(8) NULL,
    CatalogueAgreementSigned bit DEFAULT 0 NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_Organisations PRIMARY KEY(Id),
    CONSTRAINT AK_Organisations_Name UNIQUE ([Name]),
    CONSTRAINT FK_Organisations_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
);
