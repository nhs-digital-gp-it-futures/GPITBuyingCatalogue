CREATE PROCEDURE import.ImportAssociatedService
     @AssociatedServices import.AssociatedServices READONLY,
     @AssociatedCatalogueItems import.AssociatedCatalogueItems READONLY
AS
    SET NOCOUNT ON;

    DECLARE @items AS TABLE
    (
        Id nvarchar(14) NOT NULL,
        SupplierId nvarchar(6) NULL,
        [Name] nvarchar(255) NOT NULL,
        [Description] nvarchar(1000) NULL,
        OrderGuidance nvarchar(1000) NULL
    );

    INSERT INTO @items(Id, [Name], SupplierId, [Description], OrderGuidance)
         SELECT a.Id, a.[Name], gsi.Id, a.[Description], a.OrderGuidance
          FROM @AssociatedServices AS a
               CROSS APPLY import.GetSupplierId(a.Id) AS gsi;

    DECLARE @missingSuppliers AS TABLE
    (
        SupplierId nvarchar(6) PRIMARY KEY
    );

    INSERT INTO @missingSuppliers (SupplierId)
         SELECT DISTINCT i.SupplierId
           FROM @items AS i
          WHERE NOT EXISTS (SELECT * FROM dbo.Supplier AS s WHERE s.Id = i.SupplierId);

    IF EXISTS (SELECT * FROM @missingSuppliers)
    BEGIN;
        DECLARE @missingSuppliersList AS nvarchar(max) = (SELECT STRING_AGG(SupplierId, ', ') FROM @missingSuppliers);
        DECLARE @errorMessage AS nvarchar(max) = 'One or more supplier records do not exist for the following IDs: ' + @missingSuppliersList;

        THROW 51000, @errorMessage, 1;
    END;

    DECLARE @associatedServiceCatalogueItemType AS int = 3;
    DECLARE @emptyGuid AS uniqueidentifier = CAST(0x0 AS uniqueidentifier);
    DECLARE @now AS datetime = GETUTCDATE();

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, [Name], CatalogueItemTypeId, SupplierId)
             SELECT i.Id, i.[Name], @associatedServiceCatalogueItemType, i.SupplierId
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.AssociatedService AS a WHERE a.AssociatedServiceId = i.Id);

        INSERT INTO dbo.AssociatedService(AssociatedServiceId, [Description], OrderGuidance, LastUpdated, LastUpdatedBy)
             SELECT i.Id, i.[Description], i.OrderGuidance, @now, @emptyGuid
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.AssociatedService AS a WHERE a.AssociatedServiceId = i.Id);

        INSERT INTO dbo.SupplierServiceAssociation(AssociatedServiceId, CatalogueItemId)
             SELECT AssociatedServiceId, AssociatedCatalogueItemId
               FROM @AssociatedCatalogueItems AS a
              WHERE NOT EXISTS (
                    SELECT *
                      FROM dbo.SupplierServiceAssociation AS s
                     WHERE s.AssociatedServiceId = a.AssociatedServiceId
                       AND s.CatalogueItemId = a.AssociatedCatalogueItemId
                    );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
