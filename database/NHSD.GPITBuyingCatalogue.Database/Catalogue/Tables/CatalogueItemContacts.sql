CREATE TABLE catalogue.CatalogueItemContacts
(
    CatalogueItemId nvarchar(14) NOT NULL,
    SupplierContactId int NOT NULL,
    CONSTRAINT PK_CatalogueItemContacts PRIMARY KEY (CatalogueItemId, SupplierContactId),
    CONSTRAINT FK_CatalogueItemContacts_CatalogueItemId FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CatalogueItemContacts_SupplierContactId FOREIGN KEY (SupplierContactId) REFERENCES catalogue.SupplierContacts(Id),
);
