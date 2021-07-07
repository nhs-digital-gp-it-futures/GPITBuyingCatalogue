CREATE TABLE dbo.CatalogueItemCapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     Pass bit NOT NULL,
     CONSTRAINT PK_CatalogueItemCapabilityStatus PRIMARY KEY CLUSTERED (Id),
     CONSTRAINT AK_CatalogueItemCapabilityStatus UNIQUE NONCLUSTERED ([Name])
);
