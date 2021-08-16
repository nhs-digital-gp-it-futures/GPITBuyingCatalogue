CREATE TABLE catalogue.SupplierServiceAssociations
(
    AssociatedServiceId nvarchar(14) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    CONSTRAINT FK_SupplierServiceAssociations_AssociatedService FOREIGN KEY (AssociatedServiceId) REFERENCES catalogue.AssociatedServices(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_SupplierServiceAssociations_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id),
);
