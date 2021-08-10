CREATE TABLE catalogue.AssociatedServices
(
    CatalogueItemId nvarchar(14) NOT NULL,
    [Description] nvarchar(1000) NULL,
    OrderGuidance nvarchar(1000) NULL,
    LastUpdated datetime2(7) NULL,
    LastUpdatedBy uniqueidentifier NULL,
    CONSTRAINT PK_AssociatedService PRIMARY KEY (CatalogueItemId),
    CONSTRAINT FK_SupplierService_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id) ON DELETE CASCADE,
);
