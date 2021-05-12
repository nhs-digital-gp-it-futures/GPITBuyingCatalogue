CREATE PROCEDURE import.ImportAdditionalService
     @CatalogueItemId nvarchar(14),
     @ServiceName nvarchar(255),
     @ServiceSummary nvarchar(300),
     @ServiceDescription nvarchar(3000)
AS
    SET NOCOUNT ON;

    DECLARE @solutionId AS nvarchar(14) = SUBSTRING(@CatalogueItemId, 1, CHARINDEX('A', @CatalogueItemId) - 1);

    IF NOT EXISTS (SELECT * FROM dbo.Solution WHERE Id = @solutionId)
        THROW 51000, 'Parent Solution record does not exist.', 1;
        
    DECLARE @additionalServiceCatalogueItemType AS int = (SELECT CatalogueItemTypeId FROM dbo.CatalogueItemType WHERE [Name] = 'Additional Service');
    DECLARE @draftPublicationStatus AS int = (SELECT Id FROM dbo.PublicationStatus WHERE [Name] = 'Draft');
    DECLARE @emptyGuid AS uniqueidentifier = CAST(0x0 AS uniqueidentifier);
    DECLARE @now AS datetime = GETUTCDATE();
    DECLARE @supplierId AS nvarchar(6) = import.GetSupplierId(@CatalogueItemId);

    BEGIN TRANSACTION;

    BEGIN TRY
        IF NOT EXISTS (SELECT * FROM dbo.CatalogueItem WHERE CatalogueItemId = @CatalogueItemId)
            INSERT INTO dbo.CatalogueItem(CatalogueItemId, [Name], Created,
                        CatalogueItemTypeId, SupplierId, PublishedStatusId)
                 VALUES (@CatalogueItemId, @ServiceName, @now,
                        @additionalServiceCatalogueItemType, @supplierId, @draftPublicationStatus);

        IF NOT EXISTS (SELECT * FROM dbo.AdditionalService WHERE CatalogueItemId = @CatalogueItemId)
            INSERT INTO dbo.AdditionalService(CatalogueItemId, SolutionId, Summary, FullDescription,
                   LastUpdated, LastUpdatedBy)
            VALUES (@CatalogueItemId, @solutionId, @ServiceSummary, @ServiceDescription,
                   @now, @emptyGuid);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
