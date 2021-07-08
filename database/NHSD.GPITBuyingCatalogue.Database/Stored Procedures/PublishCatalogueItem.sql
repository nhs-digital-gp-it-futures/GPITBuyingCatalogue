CREATE PROCEDURE publish.PublishCatalogueItem
     @CatalogueItemId nvarchar(14)
AS
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @publishedStatus AS int = (SELECT Id FROM catalogue.PublicationStatus WHERE [Name] = 'Published');

        UPDATE catalogue.CatalogueItems
           SET PublishedStatusId = @publishedStatus
         WHERE CatalogueItemId = @CatalogueItemId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
