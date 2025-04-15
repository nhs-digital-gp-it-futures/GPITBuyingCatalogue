BEGIN TRANSACTION

DECLARE @IsCommissionedBy VARCHAR(3)
DECLARE @IsLocatedInTheGeographyOf VARCHAR(3)
SET @IsCommissionedBy = 'RE4'
SET @IsLocatedInTheGeographyOf = 'RE5'

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocations]
    ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT [or].[OrderId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode], [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
    ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @IsCommissionedBy
    AND [rel2].[RelationshipTypeId] = @IsLocatedInTheGeographyOf;

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocationRecipients]
    ([OrderId], [RecipientOdsCode], [ParentSublocationOdsCode])
SELECT [or].[OrderId], [or].[OdsCode] AS [RecipientOdsCode], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @IsCommissionedBy;

INSERT INTO [orders].[OrderItemSublocationRecipients]
    ([OrderId], [CatalogueItemId], [RecipientOdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy])
SELECT [OrderId], [CatalogueItemId], [RecipientOdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy]
FROM [ordering].[OrderItemRecipients];

COMMIT TRANSACTION;