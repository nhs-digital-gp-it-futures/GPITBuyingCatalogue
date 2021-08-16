CREATE TABLE catalogue.CatalogueItemEpicStatus
(
     Id int NOT NULL,
     [Name] nvarchar(16) NOT NULL,
     IsMet bit NOT NULL,
     CONSTRAINT PK_CatalogueItemEpicStatus PRIMARY KEY (Id),
     CONSTRAINT AK_CatalogueItemEpicStatus_Name UNIQUE ([Name]),
);
