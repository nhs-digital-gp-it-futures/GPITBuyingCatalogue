CREATE TABLE catalogue.CatalogueItemTypes
(
    Id int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    CONSTRAINT PK_CatalogueItemType PRIMARY KEY (Id),
    CONSTRAINT IX_CatalogueItemTypeName UNIQUE NONCLUSTERED ([Name]),
);
