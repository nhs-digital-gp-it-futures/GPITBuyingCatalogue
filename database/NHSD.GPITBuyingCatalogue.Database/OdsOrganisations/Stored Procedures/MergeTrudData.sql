CREATE PROCEDURE [ods_organisations].[MergeTrudData]
AS
    BEGIN TRAN
    BEGIN TRY
        -- Merge RoleTypes
        MERGE INTO ods_organisations.RoleTypes AS TARGET
        using ods_organisations.RoleTypes_Staging AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
          UPDATE SET TARGET.[Id] = SOURCE.[Id],
                     TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
          INSERT ([Id],
                  [Description])
          VALUES (SOURCE.[Id],
                  SOURCE.[Description]);
      
        -- Merge RelationshipTypes
        MERGE INTO ods_organisations.RelationshipTypes AS TARGET
        using ods_organisations.RelationshipTypes_Staging AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
          UPDATE SET TARGET.[Id] = SOURCE.[Id],
                     TARGET.[Description] = SOURCE.[Description]
        WHEN NOT MATCHED BY TARGET THEN
          INSERT ([Id],
                  [Description])
          VALUES (SOURCE.[Id],
                  SOURCE.[Description]);
      
        -- Merge OdsOrganisations
        MERGE INTO ods_organisations.OdsOrganisations AS TARGET
        using ods_organisations.OdsOrganisations_Staging AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
          UPDATE SET TARGET.[Id]           = SOURCE.[Id],
                     TARGET.[Name]         = SOURCE.[Name],
                     TARGET.[AddressLine1] = SOURCE.[AddressLine1],
                     TARGET.[AddressLine2] = SOURCE.[AddressLine2],
                     TARGET.[AddressLine3] = SOURCE.[AddressLine3],
                     TARGET.[Town]         = SOURCE.[Town],
                     TARGET.[County]       = SOURCE.[County],
                     TARGET.[Postcode]     = SOURCE.[Postcode],
                     TARGET.[Country]      = SOURCE.[Country],
                     TARGET.[IsActive]     = SOURCE.[IsActive]
        WHEN NOT MATCHED BY TARGET THEN
          INSERT ([Id], [Name], [AddressLine1], [AddressLine2], [AddressLine3], [Town], [County], [Postcode], [Country], [IsActive])
          VALUES (SOURCE.[Id],
                  SOURCE.[Name],
                  SOURCE.[AddressLine1],
                  SOURCE.[AddressLine2],
                  SOURCE.[AddressLine3],
                  SOURCE.[Town],
                  SOURCE.[County],
                  SOURCE.[Postcode],
                  SOURCE.[Country],
                  SOURCE.[IsActive]);
      
        -- Merge OrganisationRoles
        MERGE INTO ods_organisations.OrganisationRoles AS TARGET
        using ods_organisations.OrganisationRoles_Staging AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
          UPDATE SET TARGET.[Id]                = SOURCE.[Id],
                     TARGET.[OrganisationId]    = SOURCE.[OrganisationId],
                     TARGET.[RoleId]            = SOURCE.[RoleId],
                     TARGET.[IsPrimaryRole]     = SOURCE.[IsPrimaryRole]
        WHEN NOT MATCHED BY TARGET THEN
          INSERT ([Id], [OrganisationId], [RoleId], [IsPrimaryRole])
          VALUES (SOURCE.[Id],
                  SOURCE.[OrganisationId],
                  SOURCE.[RoleId],
                  SOURCE.[IsPrimaryRole]);
      
        -- Merge OrganisationRelationships
        MERGE INTO ods_organisations.OrganisationRelationships AS TARGET
        using ods_organisations.OrganisationRelationships_Staging AS SOURCE
        ON TARGET.[Id] = SOURCE.[Id]
        WHEN MATCHED THEN
          UPDATE SET TARGET.[Id]                    = SOURCE.[Id],
                     TARGET.[RelationshipTypeId]    = SOURCE.[RelationshipTypeId],
                     TARGET.[TargetOrganisationId]  = SOURCE.[TargetOrganisationId],
                     TARGET.[OwnerOrganisationId]   = SOURCE.[OwnerOrganisationId]
        WHEN NOT MATCHED BY TARGET THEN
          INSERT ([Id], [RelationshipTypeId], [TargetOrganisationId], [OwnerOrganisationId])
          VALUES (SOURCE.[Id],
                  SOURCE.[RelationshipTypeId],
                  SOURCE.[TargetOrganisationId],
                  SOURCE.[OwnerOrganisationId]);

        COMMIT TRAN
    END TRY
    BEGIN CATCH
        ROLLBACK TRAN
    END CATCH
    
    TRUNCATE TABLE ods_organisations.RoleTypes_Staging;
    TRUNCATE TABLE ods_organisations.RelationshipTypes_Staging;
    TRUNCATE TABLE ods_organisations.OdsOrganisations_Staging;
    TRUNCATE TABLE ods_organisations.OrganisationRoles_Staging;
    TRUNCATE TABLE ods_organisations.OrganisationRelationships_Staging;

    RETURN 0 
