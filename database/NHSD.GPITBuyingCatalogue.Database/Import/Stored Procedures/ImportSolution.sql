﻿CREATE PROCEDURE import.ImportSolution
     @Solutions import.Solutions READONLY,
     @Capabilities import.SolutionCapability READONLY
AS
    SET NOCOUNT ON;

    DECLARE @items AS TABLE
    (
        Id nvarchar(14) NOT NULL,
        SupplierId int NULL,
        [Name] nvarchar(255) NOT NULL,
        IsFoundation bit DEFAULT 0 NULL,
        FrameworkId nvarchar(36) NOT NULL
    );

    INSERT INTO @items(Id, [Name], SupplierId, IsFoundation, FrameworkId)
         SELECT s.Id, s.[Name], gsi.Id, s.IsFoundation, s.FrameworkId
          FROM @Solutions AS s
               CROSS APPLY import.GetSupplierId(s.Id) AS gsi;

    DECLARE @missingSuppliers AS TABLE
    (
        SupplierId int PRIMARY KEY
    );

    INSERT INTO @missingSuppliers (SupplierId)
         SELECT DISTINCT i.SupplierId
           FROM @items AS i
          WHERE NOT EXISTS (SELECT * FROM catalogue.Suppliers AS s WHERE s.Id = i.SupplierId);

    IF EXISTS (SELECT * FROM @missingSuppliers)
    BEGIN;
        DECLARE @missingSuppliersList AS nvarchar(max) = (SELECT STRING_AGG(SupplierId, ', ') FROM @missingSuppliers);
        DECLARE @errorMessage AS nvarchar(max) = 'One or more supplier records do not exist for the following IDs: ' + @missingSuppliersList;

        THROW 51000, @errorMessage, 1;
    END;

    DECLARE @noUser AS int = 0;
    DECLARE @now AS datetime = GETUTCDATE();
    DECLARE @passedFull AS int = 1;
    DECLARE @solutionCatalogueItemType AS int = 1;

    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO catalogue.CatalogueItems(Id, [Name], CatalogueItemTypeId, SupplierId)
             SELECT i.Id, i.[Name], @solutionCatalogueItemType, i.SupplierId
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM catalogue.CatalogueItems AS c WHERE c.Id = i.Id);

        INSERT INTO catalogue.Solutions(CatalogueItemId, LastUpdated, LastUpdatedBy)
             SELECT i.Id, @now, @noUser
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM catalogue.Solutions AS s WHERE s.CatalogueItemId = i.Id);

        INSERT INTO catalogue.FrameworkSolutions(FrameworkId, SolutionId, IsFoundation, LastUpdated, LastUpdatedBy)
             SELECT i.FrameworkId, i.Id, 0, @now, @noUser
               FROM @items AS i
              WHERE NOT EXISTS (SELECT * FROM catalogue.FrameworkSolutions AS f WHERE f.SolutionId = i.Id AND f.FrameworkId = i.FrameworkId);

        UPDATE f
           SET f.IsFoundation = i.IsFoundation
          FROM catalogue.FrameworkSolutions AS f
               INNER JOIN @items AS i ON i.Id = f.SolutionId
         WHERE f.FrameworkId = i.FrameworkId;

        DELETE FROM c
               FROM catalogue.CatalogueItemCapabilities AS c
                    INNER JOIN @items AS i ON i.Id = c.CatalogueItemId;

        INSERT INTO catalogue.CatalogueItemCapabilities(CatalogueItemId, CapabilityId, StatusId, LastUpdated, LastUpdatedBy)
             SELECT SolutionId, c.Id, @passedFull, @now, @noUser
               FROM @Capabilities AS cap
                    INNER JOIN catalogue.Capabilities AS c
                    ON c.CapabilityRef = cap.CapabilityRef;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
