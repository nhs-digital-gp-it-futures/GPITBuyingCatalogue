BEGIN TRANSACTION

DECLARE @OrderSublocationIsCommissionedBy VARCHAR(3)
DECLARE @OrderSublocationIsLocatedInTheGeographyOf VARCHAR(3)
SET @OrderSublocationIsCommissionedBy = 'RE4'
SET @OrderSublocationIsLocatedInTheGeographyOf = 'RE5'

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocations]
    ([OrderId], [SublocationOdsCode], [OwnerOdsCode])
SELECT DISTINCT [or].[OrderId], [rel].[OwnerOrganisationId] AS [SublocationOdsCode], [rel2].[OwnerOrganisationId] AS [OwnerOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel2]
    ON [rel].[OwnerOrganisationId] = [rel2].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy
    AND [rel2].[RelationshipTypeId] = @OrderSublocationIsLocatedInTheGeographyOf;

INSERT INTO [GPITBuyingCatalogue].[ordering].[OrderSublocationRecipients]
    ([OrderId], [RecipientOdsCode], [ParentSublocationOdsCode])
SELECT [or].[OrderId], [or].[OdsCode] AS [RecipientOdsCode], [rel].[OwnerOrganisationId] AS [ParentSublocationOdsCode]
FROM [GPITBuyingCatalogue].[ordering].[OrderRecipients] [or]
    JOIN [GPITBuyingCatalogue].[ods_organisations].[OrganisationRelationships] [rel]
    ON [or].[OdsCode] = [rel].[TargetOrganisationId]
WHERE [rel].[RelationshipTypeId] = @OrderSublocationIsCommissionedBy;

INSERT INTO [ordering].[OrderItemSublocationRecipients]
([OrderId], [CatalogueItemId], [OdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy]
)
SELECT [OrderId], [CatalogueItemId], [OdsCode], [Quantity], [DeliveryDate], [LastUpdated], [LastUpdatedBy]
FROM [ordering].[OrderItemRecipients];

COMMIT TRANSACTION;