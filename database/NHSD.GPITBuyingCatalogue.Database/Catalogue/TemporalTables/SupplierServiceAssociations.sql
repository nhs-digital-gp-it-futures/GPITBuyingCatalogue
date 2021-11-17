CREATE TABLE catalogue.SupplierServiceAssociations_History
(
    AssociatedServiceId nvarchar(14) NOT NULL,
    CatalogueItemId nvarchar(14) NOT NULL,
    LastUpdated datetime2(7) NOT NULL,
    LastUpdatedBy int NULL,
    SysStartTime datetime2(0) NOT NULL,
    SysEndTime datetime2(0) NOT NULL
);
GO

CREATE CLUSTERED COLUMNSTORE INDEX IX_SupplierServiceAssociations_History
ON catalogue.SupplierServiceAssociations_History;
GO

CREATE NONCLUSTERED INDEX IX_SupplierServiceAssociations_History_AssociatedServiceId_CatalogueItemId_Period_Columns
ON catalogue.SupplierServiceAssociations_History (SysEndTime, SysStartTime, AssociatedServiceId, CatalogueItemId);
GO
