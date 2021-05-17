CREATE PROCEDURE publish.PublishCatalogueItem
     @CatalogueItemId nvarchar(14)
AS
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    BEGIN TRY
        DECLARE @publishedStatus AS int = (SELECT Id FROM dbo.PublicationStatus WHERE [Name] = 'Published');

        UPDATE dbo.CatalogueItem
           SET PublishedStatusId = @publishedStatus
         WHERE CatalogueItemId = @CatalogueItemId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
