CREATE TABLE catalogue.CatalogueItemTypes
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT PK_CatalogueItemTypes PRIMARY KEY (Id),
    CONSTRAINT AK_CatalogueItemTypes_Name UNIQUE ([Name]),
);
