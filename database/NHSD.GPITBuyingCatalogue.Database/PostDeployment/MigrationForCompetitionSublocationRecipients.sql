BEGIN TRANSACTION

DECLARE @CompetitionSublocationIsCommissionedBy VARCHAR(3)
DECLARE @CompetitionSublocationIsLocatedInTheGeographyOf VARCHAR(3)
SET @CompetitionSublocationIsCommissionedBy = 'RE4'
SET @CompetitionSublocationIsLocatedInTheGeographyOf = 'RE5'

INSERT INTO [GPITBuyingCatalogue].[competitions].[CompetitionSublocations]
    ([CompetitionId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT [cr].[CompetitionId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode], [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
FROM [GPITBuyingCatalogue].[competitions].[CompetitionRecipients] [cr]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [cr].[OdsCode] = [rel].[TargetOrganisationId]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
    ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @CompetitionSublocationIsCommissionedBy
    AND [rel2].[RelationshipTypeId] = @CompetitionSublocationIsLocatedInTheGeographyOf
    AND [rel].[IsActive] = 1
    AND [rel2].[IsActive] = 1;

INSERT INTO [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients]
    ([CompetitionId], [ParentSublocationOdsCode], [RecipientOdsCode])
SELECT [cr].[CompetitionId], [cr].[OdsCode] AS [RecipientOdsCode], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode]
FROM [GPITBuyingCatalogue].[competitions].[CompetitionRecipients] [cr]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [cr].[OdsCode] = [rel].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @CompetitionSublocationIsCommissionedBy AND [rel].[IsActive] = 1;

INSERT INTO [competitions].[ServiceQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ServiceId],[ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity]
FROM [competitions].[ServiceQuantities] [serq]
    JOIN [competitions].[CompetitionSublocationRecipients] [csr]
    ON [serq].[CompetitionId] = [csr].[CompetitionId] AND [serq].[OdsCode] = [csr].[RecipientOdsCode];

INSERT INTO [competitions].[SolutionQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ParentSublocationOdsCode], [RecipientOdsCode], [Quantity])
SELECT [solq].[CompetitionId], [solq].[SolutionId], [solq].[OdsCode] AS [RecipientOdsCode], [solq].[Quantity], [csr].[ParentSublocationOdsCode]
FROM [competitions].[SolutionQuantities] [solq]
    JOIN [competitions].[CompetitionSublocationRecipients] [csr]
    ON [solq].[CompetitionId] = [csr].[CompetitionId] AND [solq].[OdsCode] = [csr].[RecipientOdsCode];

COMMIT TRANSACTION;