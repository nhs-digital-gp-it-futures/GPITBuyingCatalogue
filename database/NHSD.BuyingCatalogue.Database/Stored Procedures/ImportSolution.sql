CREATE PROCEDURE import.ImportSolution
     @Solutions import.Solutions READONLY,
     @Capabilities import.SolutionCapability READONLY
AS
    SET NOCOUNT ON;

    DECLARE @items AS TABLE
    (
        Id nvarchar(14) NOT NULL,
        SupplierId nvarchar(6) NULL,
        [Name] nvarchar(255) NOT NULL,
        IsFoundation bit DEFAULT 0 NULL,
        FrameworkId nvarchar(10) NOT NULL
    );

    INSERT INTO @items(Id, [Name], SupplierId, IsFoundation, FrameworkId)
         SELECT s.Id, s.[Name], gsi.Id, s.IsFoundation, s.FrameworkId
          FROM @Solutions AS s
               CROSS APPLY import.GetSupplierId(s.Id) AS gsi;

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

    DECLARE @emptyGuid AS uniqueidentifier = CAST(0x0 AS uniqueidentifier);
    DECLARE @now AS datetime = GETUTCDATE();
    DECLARE @passedFull AS int = 1;
    DECLARE @solutionCatalogueItemType AS int = 1;

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO dbo.CatalogueItem(CatalogueItemId, [Name], CatalogueItemTypeId, SupplierId)
             SELECT i.Id, i.[Name], @solutionCatalogueItemType, i.SupplierId
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.CatalogueItem AS c WHERE c.CatalogueItemId = i.Id);

        INSERT INTO dbo.Solution(Id, LastUpdated, LastUpdatedBy)
             SELECT i.Id, @now, @emptyGuid
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.Solution AS s WHERE s.Id = i.Id);

        INSERT INTO dbo.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             SELECT i.FrameworkId, i.Id, 0, @now, @emptyGuid
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM dbo.FrameworkSolutions AS f WHERE f.SolutionId = i.Id AND f.FrameworkId = i.FrameworkId);

        UPDATE f
           SET f.IsFoundation = i.IsFoundation
          FROM dbo.FrameworkSolutions AS f
               INNER JOIN @items AS i ON i.Id = f.SolutionId
         WHERE f.FrameworkId = i.FrameworkId;

        DELETE FROM s
               FROM dbo.SolutionCapability AS s
                    INNER JOIN @items AS i ON i.Id = s.SolutionId;

        INSERT INTO dbo.SolutionCapability(SolutionId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT SolutionId, c.Id, @passedFull, @now, @emptyGuid
               FROM @Capabilities AS cap
                    INNER JOIN dbo.Capability AS c
                    ON c.CapabilityRef = cap.CapabilityRef;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
