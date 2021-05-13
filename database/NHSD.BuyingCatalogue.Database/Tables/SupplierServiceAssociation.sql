CREATE TABLE dbo.SupplierServiceAssociation
(
    AssociatedServiceId nvarchar(14) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    CONSTRAINT FK_SupplierServiceAssociation_AssociatedService FOREIGN KEY (AssociatedServiceId) REFERENCES dbo.AssociatedService(AssociatedServiceId) ON DELETE CASCADE,
    CONSTRAINT FK_SupplierServiceAssociation_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES dbo.CatalogueItem(CatalogueItemId)
);
