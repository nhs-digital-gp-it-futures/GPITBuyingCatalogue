CREATE TABLE catalogue.CatalogueItems
(
    CatalogueItemId nvarchar(14) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    CatalogueItemTypeId int NOT NULL,
    SupplierId nvarchar(6) NOT NULL,
    PublishedStatusId int CONSTRAINT DF_CatalogueItem_PublishedStatus DEFAULT 1 NOT NULL,
    Created datetime2(7) CONSTRAINT DF_CatalogueItem_Created DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_CatalogueItem PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_CatalogueItem_CatalogueItemType FOREIGN KEY (CatalogueItemTypeId) REFERENCES catalogue.CatalogueItemType(CatalogueItemTypeId),
    CONSTRAINT FK_CatalogueItem_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Supplier(Id),
    CONSTRAINT FK_CatalogueItem_PublicationStatus FOREIGN KEY (PublishedStatusId) REFERENCES catalogue.PublicationStatus(Id),
    CONSTRAINT IX_CatalogueItem_Supplier_Name UNIQUE NONCLUSTERED ([SupplierId], [Name])
);
