CREATE PROCEDURE import.ImportAdditionalService
     @AdditionalServices import.AdditionalServices READONLY
AS
    SET NOCOUNT ON;

    DECLARE @items AS TABLE
    (
        Id nvarchar(14) NOT NULL,
        SolutionId AS SUBSTRING(Id, 1, CHARINDEX('A', Id) - 1),
        SupplierId nvarchar(6) NULL,
        [Name] nvarchar(255) NOT NULL,
        Summary nvarchar(300) NULL,
        [Description] nvarchar(3000) NULL
    );

    INSERT INTO @items(Id, SupplierId, [Name], Summary, [Description])
         SELECT s.Id, gsi.Id, s.[Name], s.Summary, s.[Description]
          FROM @AdditionalServices AS s
               CROSS APPLY import.GetSupplierId(s.Id) AS gsi;

    DECLARE @missingSolutions AS TABLE
    (
        Id nvarchar(14) PRIMARY KEY
    );

    INSERT INTO @missingSolutions (Id)
         SELECT DISTINCT i.SolutionId
           FROM @items AS i
          WHERE NOT EXISTS (SELECT * FROM dbo.Solution AS s WHERE s.Id = i.SolutionId);

    IF EXISTS (SELECT * FROM @missingSolutions)
    BEGIN;
        DECLARE @missingSolutionsList AS nvarchar(max) = (SELECT STRING_AGG(Id, ', ') FROM @missingSolutions);
        DECLARE @errorMessage AS nvarchar(max) = 'One or more solutions do not exist for the following IDs: ' + @missingSolutionsList;

        THROW 51000, @errorMessage, 1;
    END;
        
    DECLARE @additionalServiceCatalogueItemType AS int = 2;
    DECLARE @emptyGuid AS uniqueidentifier = CAST(0x0 AS uniqueidentifier);
    DECLARE @now AS datetime = GETUTCDATE();

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, [Name], CatalogueItemTypeId, SupplierId)
             SELECT i.Id, i.[Name], @additionalServiceCatalogueItemType, i.SupplierId
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.CatalogueItem AS c WHERE c.CatalogueItemId = i.Id);

        INSERT INTO dbo.AdditionalService(CatalogueItemId, SolutionId, Summary, FullDescription, LastUpdated, LastUpdatedBy)
            SELECT i.Id, i.SolutionId, i.Summary, i.[Description], @now, @emptyGuid
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.AdditionalService AS a WHERE a.CatalogueItemId = i.Id);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
