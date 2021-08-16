CREATE TABLE catalogue.CatalogueItemCapabilityStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     Pass bit NOT NULL,
     CONSTRAINT PK_CatalogueItemCapabilityStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CatalogueItemCapabilityStatus_Name UNIQUE ([Name]),
);
