CREATE TABLE catalogue.SupplierServiceAssociations
(
    AssociatedServiceId nvarchar(14) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    LastUpdated datetime2(7) DEFAULT GETUTCDATE() NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime datetime2(0) GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime),
    CONSTRAINT PK_SupplierServiceAssociations PRIMARY KEY (AssociatedServiceId, CatalogueItemId),
    CONSTRAINT FK_SupplierServiceAssociations_AssociatedService FOREIGN KEY (AssociatedServiceId) REFERENCES catalogue.AssociatedServices(CatalogueItemId) ON DELETE CASCADE,
    CONSTRAINT FK_SupplierServiceAssociations_CatalogueItem FOREIGN KEY (CatalogueItemId) REFERENCES catalogue.CatalogueItems(Id),
    CONSTRAINT FK_SupplierServiceAssociations_LastUpdatedBy FOREIGN KEY (LastUpdatedBy) REFERENCES users.AspNetUsers(Id),
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = catalogue.SupplierServiceAssociations_History));
