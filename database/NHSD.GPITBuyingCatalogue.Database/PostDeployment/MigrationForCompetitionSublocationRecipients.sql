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
    AND [rel2].[RelationshipTypeId] = @CompetitionSublocationIsLocatedInTheGeographyOf;

INSERT INTO [GPITBuyingCatalogue].[competitions].[CompetitionSublocationRecipients]
    ([CompetitionId], [RecipientOdsCode], [ParentSublocationOdsCode])
SELECT [cr].[CompetitionId], [cr].[OdsCode] AS [RecipientOdsCode], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode]
FROM [GPITBuyingCatalogue].[competitions].[CompetitionRecipients] [cr]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [cr].[OdsCode] = [rel].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @CompetitionSublocationIsCommissionedBy;

INSERT INTO [competitions].[ServiceQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [ServiceId], [OdsCode], [Quantity]
FROM [competitions].[ServiceQuantities];

INSERT INTO [competitions].[SolutionQuantitiesSublocationRecipients]
    ([CompetitionId], [SolutionId], [OdsCode], [Quantity])
SELECT [CompetitionId], [SolutionId], [OdsCode], [Quantity]
FROM [competitions].[SolutionQuantities];

COMMIT TRANSACTION;