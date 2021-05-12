CREATE PROCEDURE import.ImportAssociatedService
     @AssociatedServiceId nvarchar(14),
     @ServiceName nvarchar(255),
     @ServiceDescription nvarchar(1000),
     @OrderGuidance nvarchar(1000),
     @AssociatedCatalogueItems import.AssociatedCatalogueItems READONLY
AS
    SET NOCOUNT ON;

    DECLARE @supplierId AS nvarchar(6) = import.GetSupplierId(@AssociatedServiceId);

    IF NOT EXISTS (SELECT * FROM dbo.Supplier WHERE Id = @supplierId)
        THROW 51000, 'Supplier record does not exist.', 1;

    DECLARE @associatedServiceCatalogueItemType AS int = (SELECT CatalogueItemTypeId FROM dbo.CatalogueItemType WHERE [Name] = 'Associated Service');
    DECLARE @draftPublicationStatus AS int = (SELECT Id FROM dbo.PublicationStatus WHERE [Name] = 'Draft');
    DECLARE @emptyGuid AS uniqueidentifier = CAST(0x0 AS uniqueidentifier);
    DECLARE @now AS datetime = GETUTCDATE();

    BEGIN TRANSACTION;

    BEGIN TRY
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @AssociatedServiceId)
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, [Name], Created,
                        CatalogueItemTypeId, SupplierId, PublishedStatusId)
                 VALUES (@AssociatedServiceId, @ServiceName, @now,
                        @associatedServiceCatalogueItemType, @supplierId, @draftPublicationStatus);

        IF NOT EXISTS (SELECT * FROM dbo.AssociatedService WHERE AssociatedServiceId = @AssociatedServiceId)
            INSERT INTO dbo.AssociatedService(AssociatedServiceId, [Description], OrderGuidance,
                   LastUpdated, LastUpdatedBy)
            VALUES (@AssociatedServiceId, @ServiceDescription, @OrderGuidance,
                   @now, @emptyGuid);

        INSERT INTO dbo.SupplierServiceAssociation(AssociatedServiceId, CatalogueItemId)
             SELECT @AssociatedServiceId, CatalogueItemId
               FROM @AssociatedCatalogueItems AS a
              WHERE NOT EXISTS (
                    SELECT *
                      FROM dbo.SupplierServiceAssociation AS s
                     WHERE s.AssociatedServiceId = @AssociatedServiceId
                       AND s.CatalogueItemId = a.CatalogueItemId
                    );

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
