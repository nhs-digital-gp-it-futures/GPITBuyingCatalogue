CREATE TABLE catalogue.SupplierServiceAssociations
(
    AssociatedServiceId nvarchar(14) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    CONSTRAINT FK_SupplierServiceAssociation_AssociatedService FOREIGN KEY (AssociatedServiceId) REFERENCES catalogue.AssociatedServices(AssociatedServiceId) ON DELETE CASCADE,
    CONSTRAINT FK_SupplierServiceAssociation_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id),
);
