IF UPPER('$(INSERT_TEST_DATA)') = 'FALSE'
BEGIN TRANSACTION

DECLARE @CompetitionSublocationIsCommissionedBy VARCHAR(3)
DECLARE @CompetitionSublocationIsLocatedInTheGeographyOf VARCHAR(3)
SET @CompetitionSublocationIsCommissionedBy = 'RE4'
SET @CompetitionSublocationIsLocatedInTheGeographyOf = 'RE5'

INSERT INTO [GPITBuyingCatalogue].[competitions].[CompetitionSublocations]
    ([CompetitionId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT [cr].[CompetitionId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode],
                [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
    FROM [GPITBuyingCatalogue].[competitions].[CompetitionRecipients] [cr]
             JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
                  ON [cr].[OdsCode] = [rel].[TargetOrganisationId]
             JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
                  ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
    WHERE [rel].[RelationshipTypeId] = @CompetitionSublocationIsCommissionedBy
      AND [rel2].[RelationshipTypeId] = @CompetitionSublocationIsLocatedInTheGeographyOf
      AND [rel].[IsActive] = 1
      AND [rel2].[IsActive] = 1
      AND NOT EXISTS (
        SELECT 1
            FROM [GPITBuyingCatalogue].[competitions].[CompetitionSublocations] [cs]
            WHERE [cs].[CompetitionId] = [cr].[CompetitionId]
              AND [cs].[SublocationOdsCode] = [rel].[OwnerOrganisationId]
              AND [cs].[OwnerOdsCode] = [rel2].[OwnerOrganisationId]);

INSERT INTO [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients]
    ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode])
SELECT [cr].[CompetitionId], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode],
       [cr].[OdsCode] AS [RecipientOdsCode]
    FROM [GPITBuyingCatalogue].[competitions].[CompetitionRecipients] [cr]
             JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
                  ON [cr].[OdsCode] = [rel].[TargetOrganisationId]
    WHERE [rel].[RelationshipTypeId] = @CompetitionSublocationIsCommissionedBy
      AND [rel].[IsActive] = 1
      AND NOT EXISTS (
        SELECT 1
            FROM [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients] [csr]
            WHERE [csr].[CompetitionId] = [cr].[CompetitionId]
              AND [csr].[ParentSublocationOdsCode] = [rel].[OwnerOrganisationId]
              AND [csr].[RecipientOdsCode] = [cr].[OdsCode]);

INSERT INTO [competitions].[ServiceQuantitiesSublocationRecipients]
([CompetitionId], [SolutionId], [ServiceId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [serq].[CompetitionId], [serq].[SolutionId], [serq].[ServiceId], [csr].[ParentSublocationOdsCode],
       [csr].[RecipientOdsCode], [serq].[Quantity]
    FROM [competitions].[ServiceQuantities] [serq]
             JOIN [competitions].[CompetitionSublocationRecipients] [csr]
                  ON [serq].[CompetitionId] = [csr].[CompetitionId] AND [serq].[OdsCode] = [csr].[RecipientOdsCode]
    WHERE NOT EXISTS (
        SELECT 1
            FROM [competitions].[ServiceQuantitiesSublocationRecipients] [sqsr]
            WHERE [sqsr].[CompetitionId] = [serq].[CompetitionId]
              AND [sqsr].[SolutionId] = [serq].[SolutionId]
              AND [sqsr].[ServiceId] = [serq].[ServiceId]
              AND [sqsr].[ParentSublocationOdsCode] = [csr].[ParentSublocationOdsCode]
              AND [sqsr].[RecipientOdsCode] = [csr].[RecipientOdsCode]);

INSERT INTO [competitions].[SolutionQuantitiesSublocationRecipients]
([CompetitionId], [SolutionId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [solq].[CompetitionId], [solq].[SolutionId], [csr].[ParentSublocationOdsCode], [csr].[RecipientOdsCode],
       [solq].[Quantity]
    FROM [competitions].[SolutionQuantities] [solq]
             JOIN [competitions].[CompetitionSublocationRecipients] [csr]
                  ON [solq].[CompetitionId] = [csr].[CompetitionId] AND [solq].[OdsCode] = [csr].[RecipientOdsCode]
    WHERE NOT EXISTS (
        SELECT 1
            FROM [competitions].[SolutionQuantitiesSublocationRecipients] [sqsr]
            WHERE [sqsr].[CompetitionId] = [solq].[CompetitionId]
              AND [sqsr].[SolutionId] = [solq].[SolutionId]
              AND [sqsr].[ParentSublocationOdsCode] = [csr].[ParentSublocationOdsCode]
              AND [sqsr].[RecipientOdsCode] = [csr].[RecipientOdsCode]);

COMMIT TRANSACTION;
