IF UPPER('$(INSERT_TEST_DATA)') = 'TRUE' AND NOT EXISTS (SELECT * FROM catalogue.SupplierServiceAssociations)
BEGIN
    -- (10000-001) Emis Web GP
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-002', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-003', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-004', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-005', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-006', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-007', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-008', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-036', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-037', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-038', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-039', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-040', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-041', '10000-001', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-069', '10000-001', GETDATE()
    
    -- (10000-002) Anywhere Consult
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-002', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-003', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-009', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-036', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-039', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-042', '10000-002', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-041', '10000-002', GETDATE()
    
    -- (10000-054) - Online and Video Consult
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-001', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-002', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-003', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-036', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-039', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-042', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-041', '10000-054', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-141', '10000-054', GETDATE()
    
    -- (10000-062) Video Consult
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-001', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-002', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-003', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-036', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-039', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-042', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-041', '10000-062', GETDATE()
    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '10000-S-141', '10000-062', GETDATE()

    INSERT catalogue.SupplierServiceAssociations ( AssociatedServiceId, CatalogueItemId, LastUpdated ) SELECT '100000-S-001', '100000-001', GETDATE()
END;
