CREATE TABLE catalogue.CatalogueItems
(
    Id nvarchar(14) NOT NULL,
    [Name] nvarchar(255) NOT NULL,
    CatalogueItemTypeId int NOT NULL,
    SupplierId int NOT NULL,
    PublishedStatusId int CONSTRAINT DF_CatalogueItem_PublishedStatus DEFAULT 1 NOT NULL,
    Created datetime2(7) CONSTRAINT DF_CatalogueItem_Created DEFAULT GETUTCDATE() NOT NULL,
    CONSTRAINT PK_CatalogueItems PRIMARY KEY (Id),
    CONSTRAINT FK_CatalogueItems_CatalogueItemType FOREIGN KEY (CatalogueItemTypeId) REFERENCES catalogue.CatalogueItemTypes(Id),
    CONSTRAINT FK_CatalogueItems_Supplier FOREIGN KEY (SupplierId) REFERENCES catalogue.Suppliers(Id),
    CONSTRAINT FK_CatalogueItems_PublicationStatus FOREIGN KEY (PublishedStatusId) REFERENCES catalogue.PublicationStatus(Id),
    CONSTRAINT AK_CatalogueItems_Supplier_Name UNIQUE (SupplierId, [Name]),
);
